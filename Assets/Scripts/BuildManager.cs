using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class PlacedStructure
{
    public GameObject obj;
    public BuildableData data;

    public PlacedStructure(GameObject o, BuildableData d)
    {
        obj = o;
        data = d;
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
    private Quaternion currentRotation = Quaternion.identity;
    private Stack<PlacedStructure> placedObjects = new Stack<PlacedStructure>();
    private List<GameObject> highlightInstances = new List<GameObject>();
    private HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();
    private List<GameObject> allPlacedObjects = new List<GameObject>();
    private bool isPlacing = false;
    private bool ratushaBuilt = false;
    private Dictionary<BuildableData, int> buildCounts = new Dictionary<BuildableData, int>();
    private HashSet<BuildableData> uniqueBuiltTypes = new HashSet<BuildableData>();
    public void StartWallPlacement() => StartPlacing(wallData);
    public void StartConcreteWallPlacement() => StartPlacing(concreteWallData);
    public void StartXPGeneratorPlacement() => StartPlacing(buildingXPGeneratorData);
    public void StartRatushaPlacement() => StartPlacing(buildingRatushaData);
    public void StartSkladPlacement() => StartPlacing(buildingSkladData);
    public void StartCannonPlacement() => StartPlacing(weaponCannonData);
    public void StartCrossbowPlacement() => StartPlacing(weaponCrossbowData);
    public bool CanCurrentlyPlace()
    {
        // Если размещается ратуша — всегда можно
        if (currentData == buildingRatushaData)
            return true;

        // Остальные здания — только если ратуша уже есть
        return ratushaBuilt;
    }

    public bool IsRatushaBuilt()
    {
        return ratushaBuilt;
    }

    private void StartPlacing(BuildableData data)
    {
        // Блокируем повторную постройку ратуши
        if (data == buildingRatushaData && ratushaBuilt)
        {
            Debug.Log("Ратуша уже построена. Нельзя построить вторую.");
            return;
        }

        // Блокируем попытку построить всё, кроме ратуши, до её постройки
        if (!ratushaBuilt && data != buildingRatushaData)
        {
            Debug.Log("Сначала постройте ратушу.");
            return;
        }

        // Проверка лимита
        if (data.buildLimit > 0 && buildCounts.TryGetValue(data, out int count) && count >= data.buildLimit)
        {
            Debug.Log($"Превышен лимит построек для {data.prefab.name}. Максимум: {data.buildLimit}");
            AchievementManager.Instance.IncrementProgress("Не все сразу", 1);
            return;
        }

        currentData = data;
        currentPrefab = data.prefab;
        isPlacing = true;
        currentRotation = Quaternion.identity;

        if (previewObject != null)
            Destroy(previewObject);

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

        Vector3 worldPos = GetCenterWorldPosition(cell, currentData.sizeX, currentData.sizeZ, currentRotation);
        worldPos.y = currentPrefab.transform.position.y;

        previewObject.transform.position = worldPos;
        previewObject.transform.rotation = currentRotation;

        List<Vector2Int> highlightCells = GetOccupiedCells(worldPos, currentData.sizeX, currentData.sizeZ, currentRotation);
        bool canPlace = CanPlaceAt(highlightCells);
        UpdateHighlightVisuals(highlightCells, canPlace);

        if (Input.GetMouseButtonDown(1))
        {
            currentRotation *= Quaternion.Euler(0, 90, 0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (!canPlace)
            {
                Debug.Log("Нельзя строить: есть занятые клетки.");
                return;
            }

            if (!CurrencyManager.Instance.HasEnough(currentData.cost))
            {
                Debug.Log("Недостаточно рублей для строительства.");
                return;
            }

            CurrencyManager.Instance.Spend(currentData.cost);

            GameObject placed = Instantiate(currentPrefab, worldPos, currentRotation);
            foreach (var pos in highlightCells)
                occupiedCells.Add(pos);

            placedObjects.Push(new PlacedStructure(placed, currentData));
            allPlacedObjects.Add(placed);

            //счетчик
            if (!buildCounts.ContainsKey(currentData))
                buildCounts[currentData] = 0;
            buildCounts[currentData]++;

            // Добавляем в список уникальных построек
            if (!uniqueBuiltTypes.Contains(currentData))
            {
                uniqueBuiltTypes.Add(currentData);
                AchievementManager.Instance.IncrementProgress("Разнообразие", 1);
            }

            if (currentData == wallData || currentData == concreteWallData)
            {
                AchievementManager.Instance.IncrementProgress("Моя территория!", 1);
            }

            if (currentData == buildingRatushaData)
            {
                ratushaBuilt = true;
                Debug.Log("Ратуша построена. Доступны остальные постройки.");
                AchievementManager.Instance.IncrementProgress("Начало", 1);
                AchievementManager.Instance.IncrementProgress("Как похорошела Москва", 1);

                SidebarController sidebar = FindObjectOfType<SidebarController>();
                if (sidebar != null)
                {
                    sidebar.UpdateBuildingButtons();
                }
            }

            if (currentData == weaponCannonData)
            {
                AchievementManager.Instance.IncrementProgress("К оружию!", 1);
            }

            if (currentData == weaponCrossbowData)
            {
                AchievementManager.Instance.IncrementProgress("Воздушная угроза", 1);
            }

            EndPlacement();
        }
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
                Vector2Int gridCell = new Vector2Int(cellX, cellZ);

                if (!cells.Contains(gridCell))
                    cells.Add(gridCell);
            }
        }

        return cells;
    }

    private bool CanPlaceAt(List<Vector2Int> cellsToCheck)
    {
        foreach (var pos in cellsToCheck)
        {
            if (occupiedCells.Contains(pos))
                return false;
        }
        return true;
    }

    private void UpdateHighlightVisuals(List<Vector2Int> cells, bool isPlaceable)
    {
        foreach (var obj in highlightInstances)
            Destroy(obj);
        highlightInstances.Clear();

        foreach (var cellPos in cells)
        {
            GameObject highlight = Instantiate(highlightPrefab);
            highlight.transform.position = new Vector3(cellPos.x + 0.5f, 0.01f, cellPos.y + 0.5f);
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
        Destroy(last.obj);
        allPlacedObjects.Remove(last.obj);

        if (last.data != null)
        {
            CurrencyManager.Instance.Add(last.data.cost);
        }

        if (last.data != null && buildCounts.ContainsKey(last.data))
        {
            buildCounts[last.data] = Mathf.Max(0, buildCounts[last.data] - 1);
            AchievementManager.Instance.IncrementProgress("Отставить!", 1);
        }

        Debug.Log("Откат последнего действия.");
    }

    private void EndPlacement()
    {
        isPlacing = false;

        if (previewObject != null)
            Destroy(previewObject);
        previewObject = null;

        foreach (var obj in highlightInstances)
            Destroy(obj);
        highlightInstances.Clear();

        currentPrefab = null;
    }

    public List<GameObject> GetAllPlacedObjects()
    {
        return allPlacedObjects;
    }

    public void ClearBuildData()
    {
        buildCounts.Clear();
        occupiedCells.Clear();
        foreach (var obj in allPlacedObjects)
            Destroy(obj);
        allPlacedObjects.Clear();
    }

    public void PlaceLoadedObject(PlacedObjectData data)
    {
        GameObject prefab = GetPrefabByName(data.prefabName);
        if (prefab != null)
        {
            GameObject placed = Instantiate(prefab, data.position, data.rotation);

            BuildingState state = placed.GetComponent<BuildingState>();
            if (state != null)
            {
                state.currentLevel = data.buildingLevel;
                state.currentHealth = data.buildingHealth;
            }

            allPlacedObjects.Add(placed);

            List<Vector2Int> cells = GetOccupiedCells(data.position, data.sizeX, data.sizeZ, data.rotation);
            foreach (var cell in cells)
                occupiedCells.Add(cell);

            if (data.prefabName == buildingRatushaData.prefab.name)
            {
                ratushaBuilt = true;

                var ratushaState = placed.GetComponent<BuildingState>();
                TownhallManager.Instance.SetLevel(ratushaState.currentLevel);
            }
        }
    }


    public void SetRatushaBuilt(bool built)
    {
        ratushaBuilt = built;
    }

    private GameObject GetPrefabByName(string name)
    {
        if (name == wallData.prefab.name) return wallData.prefab;
        if (name == concreteWallData.prefab.name) return concreteWallData.prefab;
        if (name == buildingXPGeneratorData.prefab.name) return buildingXPGeneratorData.prefab;
        if (name == buildingRatushaData.prefab.name) return buildingRatushaData.prefab;
        if (name == buildingSkladData.prefab.name) return buildingSkladData.prefab;
        if (name == weaponCannonData.prefab.name) return weaponCannonData.prefab;
        if (name == weaponCrossbowData.prefab.name) return weaponCrossbowData.prefab;
        return null;
    }

    private BuildableData GetBuildableDataByPrefab(string prefabName)
    {
        if (prefabName == wallData.prefab.name) return wallData;
        if (prefabName == concreteWallData.prefab.name) return concreteWallData;
        if (prefabName == buildingXPGeneratorData.prefab.name) return buildingXPGeneratorData;
        if (prefabName == buildingRatushaData.prefab.name) return buildingRatushaData;
        if (prefabName == buildingSkladData.prefab.name) return buildingSkladData;
        if (prefabName == weaponCannonData.prefab.name) return weaponCannonData;
        if (prefabName == weaponCrossbowData.prefab.name) return weaponCrossbowData;
        return null;
    }
}