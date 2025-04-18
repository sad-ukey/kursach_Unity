using UnityEngine;

public class BuildUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject buildMenuPanel;       // Ссылка на BuildMenuPanel
    public GameObject openMenuButton;       // Ссылка на OpenBuildMenuButton

    [Header("Building Prefabs")]
    public GameObject prefab1x1;
    public GameObject prefab2x1;
    public GameObject prefab2x2;

    private BuildingSpawner spawner;

    void Awake()
    {
        // Скрываем панель при старте
        buildMenuPanel.SetActive(false);

        // Находим/создаём спавнер
        spawner = FindObjectOfType<BuildingSpawner>();
        if (spawner == null)
        {
            GameObject go = new GameObject("Spawner");
            spawner = go.AddComponent<BuildingSpawner>();
            // Не забудьте в инспекторе проставить buildingPrefab (базовый)
        }
    }

    // Привязать к кнопке OpenBuildMenuButton → OnClick() → ToggleMenu()
    public void ToggleMenu()
    {
        buildMenuPanel.SetActive(!buildMenuPanel.activeSelf);
    }

    // Методы для кнопок внутри меню:
    public void OnSpawn1x1() => Spawn(prefab1x1);
    public void OnSpawn2x1() => Spawn(prefab2x1);
    public void OnSpawn2x2() => Spawn(prefab2x2);

    private void Spawn(GameObject prefab)
    {
        // Спауним через спавнер
        spawner.buildingPrefab = prefab;
        spawner.SpawnBuilding();
        // Закрываем меню
        buildMenuPanel.SetActive(false);
    }
}
