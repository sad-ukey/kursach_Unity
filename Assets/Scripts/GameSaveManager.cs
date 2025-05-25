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

    public void SaveGame()
    {
        SaveData save = new SaveData();

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

        Debug.Log("Игра загружена.");
    }
}