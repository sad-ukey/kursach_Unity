using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{

    [Tooltip("Префаб вашей постройки без компонента BuildingDrag")]
    public GameObject buildingPrefab;

    private BuildManager buildManager;
    private void Start()
    {
        buildManager = FindObjectOfType<BuildManager>();
    }

    // Этот метод вызывается по нажатию UI-кнопки
    public void SpawnBuilding()
    {
        if (buildingPrefab == null)
        {
            Debug.LogError("Префаб не задан!");
            return;
        }

        // Проверка: если не ратуша и ратуша ещё не построена — не строим
        string prefabName = buildingPrefab.name;


        if (prefabName != buildManager.buildingRatushaData.prefab.name && !buildManager.IsRatushaBuilt())
        {
            Debug.Log("Нельзя строить до постройки ратуши.");
            return;
        }
        // Спавним модель в любой начальной точке (например, (0,0,0))
        Vector3 spawnPosition = Vector3.zero;
        GameObject newBuilding = Instantiate(buildingPrefab, spawnPosition, Quaternion.identity);

        // Только здесь добавляем функциональность drag&drop
        newBuilding.AddComponent<BuildingDrag>();
    }
    
}
