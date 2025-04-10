using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    public GameObject buildingPrefab;  // Префаб постройки
    
    // Метод, вызываемый при нажатии кнопки UI
    public void SpawnBuilding()
    {
        // Спавним постройку в точке (0,0,0). При gridOrigin = (0,0,0) это соответствует нижней левой ячейке.
        Vector3 spawnPosition = Vector3.zero;
        GameObject newBuilding = Instantiate(buildingPrefab, spawnPosition, Quaternion.identity);
        
        // Если BuildingDrag не добавлен на префаб, добавляем его здесь
        if (newBuilding.GetComponent<BuildingDrag>() == null)
            newBuilding.AddComponent<BuildingDrag>();
    }
}
