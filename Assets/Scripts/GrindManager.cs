using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 20;
    public int gridHeight = 20;
    public float cellSize = 1f;
    public Vector3 gridOrigin = Vector3.zero;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
        // Вертикальные линии (ось Z)
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = gridOrigin + new Vector3(x * cellSize, 0, 0);
            Vector3 end = start + new Vector3(0, 0, gridHeight * cellSize);
            Gizmos.DrawLine(start, end);
        }
        
        // Горизонтальные линии (ось X)
        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 start = gridOrigin + new Vector3(0, 0, z * cellSize);
            Vector3 end = start + new Vector3(gridWidth * cellSize, 0, 0);
            Gizmos.DrawLine(start, end);
        }
    }
    
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - gridOrigin.x) / cellSize);
        int z = Mathf.FloorToInt((worldPosition.z - gridOrigin.z) / cellSize);
        return new Vector2Int(x, z);
    }
    
    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        float x = gridOrigin.x + gridPosition.x * cellSize + cellSize / 2;
        float z = gridOrigin.z + gridPosition.y * cellSize + cellSize / 2;
        return new Vector3(x, gridOrigin.y, z);
    }
}
