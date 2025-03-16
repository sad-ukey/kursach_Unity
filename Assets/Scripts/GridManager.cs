using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Количество ячеек по оси X (ширина) и Z (глубина)
    public int gridWidth = 10;
    public int gridHeight = 10;
    
    // Размер каждой ячейки
    public float cellSize = 1f;
    
    // Начало координат сетки (можно сместить, если поле не начинается от (0,0,0))
    public Vector3 gridOrigin = Vector3.zero;
    
    // Отрисовка линий сетки в редакторе
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
        // Рисуем линии вдоль оси Z для каждого столбца
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = gridOrigin + new Vector3(x * cellSize, 0, 0);
            Vector3 end = gridOrigin + new Vector3(x * cellSize, 0, gridHeight * cellSize);
            Gizmos.DrawLine(start, end);
        }
        
        // Рисуем линии вдоль оси X для каждой строки
        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 start = gridOrigin + new Vector3(0, 0, z * cellSize);
            Vector3 end = gridOrigin + new Vector3(gridWidth * cellSize, 0, z * cellSize);
            Gizmos.DrawLine(start, end);
        }
    }
    
    // Преобразование мировой позиции в координаты сетки (ячейка)
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - gridOrigin.x) / cellSize);
        int z = Mathf.FloorToInt((worldPosition.z - gridOrigin.z) / cellSize);
        return new Vector2Int(x, z);
    }
    
    // Получение центра ячейки в мировой позиции по координатам сетки
    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        float x = gridOrigin.x + gridPosition.x * cellSize + cellSize / 2;
        float z = gridOrigin.z + gridPosition.y * cellSize + cellSize / 2;
        return new Vector3(x, gridOrigin.y, z);
    }
}
