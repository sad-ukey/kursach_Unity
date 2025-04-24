using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[System.Serializable]
public class BuildableData
{
    public GameObject prefab;
    public int sizeX = 1;
    public int sizeZ = 1;
}

public class PlacedStructure
{
    public GameObject obj;
    public List<Vector2Int> occupiedCells;

    public PlacedStructure(GameObject o, List<Vector2Int> cells)
    {
        obj = o;
        occupiedCells = cells;
    }
}

public class BuildManager : MonoBehaviour
{
    [Header("Постройки")]
    public BuildableData wallData;
    public BuildableData concreteWallData;
    public BuildableData buildingXPGeneratorData;
    public BuildableData buildingRatushaData;
    public BuildableData buildingSkladData;
    public BuildableData weaponCannonData;
    public BuildableData weaponCrossbowData;

    [Header("Вспомогательные объекты")]
    public GameObject highlightPrefab;

    [Header("Слой для сетки")]
    public LayerMask gridLayer;

    [Header("Параметры")]
    public float cellSize = 1f;

    private BuildableData currentData;
    private GameObject previewObject;
    private GameObject currentPrefab;
    private List<GameObject> highlightInstances = new List<GameObject>();

    private bool isPlacing = false;
    private Quaternion currentRotation = Quaternion.identity;

    private HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();
    private Stack<PlacedStructure> placedObjects = new Stack<PlacedStructure>();
    private bool canPlace = true;

    // Методы запуска
    public void StartWallPlacement() => StartPlacing(wallData);
    public void StartConcreteWallPlacement() => StartPlacing(concreteWallData);
    public void StartXPGeneratorPlacement() => StartPlacing(buildingXPGeneratorData);
    public void StartRatushaPlacement() => StartPlacing(buildingRatushaData);
    public void StartSkladPlacement() => StartPlacing(buildingSkladData);
    public void StartCannonPlacement() => StartPlacing(weaponCannonData);
    public void StartCrossbowPlacement() => StartPlacing(weaponCrossbowData);

    private void StartPlacing(BuildableData data)
    {
        currentData = data;
        isPlacing = true;
        currentRotation = Quaternion.identity;
        currentPrefab = data.prefab;

        if (previewObject != null) Destroy(previewObject);

        previewObject = Instantiate(currentPrefab);
        previewObject.GetComponent<Collider>().enabled = false;
        SetTransparent(previewObject, true);
    }

    private void Update()
    {
        if (!isPlacing || currentPrefab == null)
            return;

        Vector3Int cell = GetMouseCell();
        if (cell == Vector3Int.one * int.MinValue)
            return;

        Vector3 centerPos = new Vector3(cell.x, 0, cell.z);
        List<Vector2Int> checkCells = GetOccupiedCells(centerPos, currentData.sizeX, currentData.sizeZ, currentRotation);

        canPlace = true;
        foreach (var c in checkCells)
        {
            if (occupiedCells.Contains(c))
            {
                canPlace = false;
                break;
            }
        }

        // Центр объекта
        Vector3 worldPos = GetCenterWorldPosition(cell, currentData.sizeX, currentData.sizeZ, currentRotation);
        worldPos.y = currentPrefab.transform.position.y;

        previewObject.transform.position = worldPos;
        previewObject.transform.rotation = currentRotation;

        UpdateHighlightVisuals(checkCells, canPlace);

        // Поворот
        if (Input.GetMouseButtonDown(1))
        {
            currentRotation *= Quaternion.Euler(0, 90, 0);
        }

        // Установка
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (!canPlace)
            {
                Debug.Log("Нельзя строить: хотя бы одна клетка занята.");
                return;
            }

            GameObject placed = Instantiate(currentPrefab, worldPos, currentRotation);
            foreach (var c in checkCells)
                occupiedCells.Add(c);

            placedObjects.Push(new PlacedStructure(placed, checkCells));
            EndPlacement();
        }
    }

    private void EndPlacement()
    {
        isPlacing = false;

        if (previewObject != null) Destroy(previewObject);
        previewObject = null;

        foreach (var obj in highlightInstances)
            Destroy(obj);
        highlightInstances.Clear();

        currentPrefab = null;
    }

    private Vector3Int GetMouseCell()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, gridLayer))
        {
            float x = Mathf.Round(hit.point.x / cellSize);
            float z = Mathf.Round(hit.point.z / cellSize);
            return new Vector3Int((int)x, 0, (int)z);
        }
        return Vector3Int.one * int.MinValue;
    }

    private List<Vector2Int> GetOccupiedCells(Vector3 center, int sizeX, int sizeZ, Quaternion rotation)
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                Vector3 offset = new Vector3(x - sizeX / 2 + 0.5f, 0, z - sizeZ / 2 + 0.5f);
                Vector3 worldOffset = rotation * offset;
                Vector3 worldPos = center + worldOffset;

                int cellX = Mathf.RoundToInt(worldPos.x);
                int cellZ = Mathf.RoundToInt(worldPos.z);
                Vector2Int cell = new Vector2Int(cellX, cellZ);

                if (!cells.Contains(cell))
                    cells.Add(cell);
            }
        }

        return cells;
    }

    private Vector3 GetCenterWorldPosition(Vector3Int baseCell, int sizeX, int sizeZ, Quaternion rotation)
    {
        Vector3 centerOffset = Vector3.zero;

        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                Vector3 localOffset = new Vector3(x - sizeX / 2 + 0.5f, 0, z - sizeZ / 2 + 0.5f);
                centerOffset += rotation * localOffset;
            }
        }

        centerOffset /= (sizeX * sizeZ);
        return new Vector3(baseCell.x, 0, baseCell.z) + centerOffset;
    }

    private void SetTransparent(GameObject obj, bool transparent)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = rend.material;
            Color color = mat.color;
            color.a = transparent ? 0.5f : 1f;
            mat.color = color;
        }
    }

    private void UpdateHighlightVisuals(List<Vector2Int> cells, bool isPlaceable)
    {
        foreach (var obj in highlightInstances)
            Destroy(obj);
        highlightInstances.Clear();

        foreach (var cell in cells)
        {
            GameObject highlight = Instantiate(highlightPrefab);
            highlight.transform.position = new Vector3(cell.x + 0.5f, 0.01f, cell.y + 0.5f);
            highlight.transform.rotation = Quaternion.Euler(90, 0, 0);
            SetHighlightColorOnObject(highlight, isPlaceable ? Color.green : Color.red);
            highlightInstances.Add(highlight);
        }
    }

    private void SetHighlightColorOnObject(GameObject obj, Color color)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            Color c = color;
            c.a = 0.5f;
            rend.material.color = c;
        }
    }

    public void CancelPlacement()
    {
        EndPlacement();
        Debug.Log("Строительство отменено.");
    }

    public void UndoLastPlacement()
    {
        if (placedObjects.Count == 0)
        {
            Debug.Log("Нет объектов для отката.");
            return;
        }

        PlacedStructure last = placedObjects.Pop();
        foreach (var c in last.occupiedCells)
            occupiedCells.Remove(c);

        Destroy(last.obj);
        Debug.Log("Откат последнего действия.");
    }
}
