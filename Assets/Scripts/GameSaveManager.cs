using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSaveManager : MonoBehaviour
{
    private BuildManager buildManager;

    private void Awake()
    {
        buildManager = FindObjectOfType<BuildManager>();
    }

    private void Start()
    {
        StartCoroutine(DelayedLoad());
    }

    private IEnumerator DelayedLoad()
    {
        yield return null;
    }

    private SaveData waveBackupData;

    public void SaveWaveCheckpoint(int currentWaveIndex)
    {
        waveBackupData = new SaveData();
        waveBackupData.savedMoney = CurrencyManager.Instance.GetCurrentMoney();
        waveBackupData.currentWaveIndex = currentWaveIndex;

        foreach (var obj in buildManager.GetAllPlacedObjects())
        {
            var size = obj.GetComponent<BuildableSize>();
            if (size == null) continue;

            waveBackupData.placedObjects.Add(new PlacedObjectData
            {
                prefabName = obj.name.Replace("(Clone)", "").Trim(),
                position = obj.transform.position,
                rotation = obj.transform.rotation,
                sizeX = size.sizeX,
                sizeZ = size.sizeZ,
                buildingLevel = obj.GetComponent<BuildingState>()?.currentLevel ?? 1,
                buildingHealth = obj.GetComponent<BuildingState>()?.currentHealth ?? 100f
            });
        }

        Debug.Log("Сохранена контрольная точка волны: " + currentWaveIndex);
    }

    public void LoadWaveCheckpoint()
    {
        if (waveBackupData == null)
        {
            Debug.LogWarning("Контрольная точка волны не найдена.");
            return;
        }

        buildManager.ClearBuildData();

        foreach (var data in waveBackupData.placedObjects)
        {
            buildManager.PlaceLoadedObject(data);
        }

        Debug.Log("Загружено состояние базы после волны " + waveBackupData.currentWaveIndex);
    }
    public void SaveGame()
    {
        var waveSpawner = FindObjectOfType<WaveSpawner>();
        if (waveSpawner != null && waveSpawner.IsWaveActive())
        {
            Debug.LogWarning("Нельзя сохранить игру во время боя.");
            return;
        }
        SaveData save = new SaveData();


        if (waveSpawner != null)
        {
            save.currentWaveIndex = waveSpawner.GetCurrentWaveIndex();
        }

        save.savedMoney = CurrencyManager.Instance.GetCurrentMoney();
        save.isRatushaBuilt = buildManager.IsRatushaBuilt();

        foreach (var obj in buildManager.GetAllPlacedObjects())
        {
            var size = obj.GetComponent<BuildableSize>();
            if (size == null) continue;

            save.placedObjects.Add(new PlacedObjectData
            {
                prefabName = obj.name.Replace("(Clone)", "").Trim(),
                position = obj.transform.position,
                rotation = obj.transform.rotation,
                sizeX = size.sizeX,
                sizeZ = size.sizeZ,
                buildingLevel = obj.GetComponent<BuildingState>()?.currentLevel ?? 1,
                buildingHealth = obj.GetComponent<BuildingState>()?.currentHealth ?? 100f
            });
        }

        save.placedCountList = AchievementManager.Instance.GetAchievementProgressList();


        SaveSystem.Save(save);
    }

    public void LoadGame()
    {
        var waveSpawner = FindObjectOfType<WaveSpawner>();
        if (waveSpawner != null && waveSpawner.IsWaveActive())
        {
            Debug.LogWarning("Нельзя загрузить во время активной волны.");
            return;
        }
        SaveData save = SaveSystem.Load();
        if (save == null)
        {
            Debug.LogWarning("Сохранение не найдено.");
            return;
        }

        CurrencyManager.Instance.LoadMoney(save.savedMoney);
        buildManager.ClearBuildData();

        foreach (var data in save.placedObjects)
        {
            buildManager.PlaceLoadedObject(data);
        }

        if (save.placedCountList != null && save.placedCountList.Count > 0)
        {
            AchievementManager.Instance.LoadProgressFromList(save.placedCountList);
        }

        buildManager.SetRatushaBuilt(save.isRatushaBuilt);

        if (waveSpawner != null)
        {
            waveSpawner.SetCurrentWaveIndex(save.currentWaveIndex);
        }

        Debug.Log("Игра загружена.");
    }
}