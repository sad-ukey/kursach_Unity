using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    [Tooltip("Префаб вашей постройки без компонента BuildingDrag")]
    public GameObject buildingPrefab;

    // Этот метод вызывается по нажатию UI-кнопки
    public void SpawnBuilding()
    {
        // Спавним модель в любой начальной точке (например, (0,0,0))
        Vector3 spawnPosition = Vector3.zero;
        GameObject newBuilding = Instantiate(buildingPrefab, spawnPosition, Quaternion.identity);

        // Только здесь добавляем функциональность drag&drop
        newBuilding.AddComponent<BuildingDrag>();
    }
}
