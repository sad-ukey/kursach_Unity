using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    [Header("Префабы построек")]
    public GameObject wallPrefab;
    public GameObject concreteWallPrefab;
    public GameObject buildingXPGeneratorPrefab;
    public GameObject buildingRatushaPrefab;
    public GameObject buildingSkladPrefab;
    public GameObject weaponCannonPrefab;
    public GameObject weaponCrossbowPrefab;

    [Header("Слой для сетки")]
    public LayerMask gridLayer;

    [Header("Параметры")]
    public float cellSize = 1f;

    private GameObject previewObject;
    private GameObject currentPrefab;
    private GameObject lastPlacedObject;
    private bool isPlacing = false;
    private Quaternion currentRotation = Quaternion.identity;

    // Методы запуска размещения объектов
    public void StartWallPlacement()
    {
        StartPlacing(wallPrefab);
        Debug.Log("Режим строительства: Частокол");
    }

    public void StartConcreteWallPlacement()
    {
        StartPlacing(concreteWallPrefab);
        Debug.Log("Режим строительства: Бетонный забор");
    }

    public void StartXPGeneratorPlacement()
    {
        StartPlacing(buildingXPGeneratorPrefab);
        Debug.Log("Строительство: Генератор опыта");
    }

    public void StartRatushaPlacement()
    {
        StartPlacing(buildingRatushaPrefab);
        Debug.Log("Строительство: Ратуша");
    }

    public void StartSkladPlacement()
    {
        StartPlacing(buildingSkladPrefab);
        Debug.Log("Строительство: Склад");
    }

    public void StartCannonPlacement()
    {
        StartPlacing(weaponCannonPrefab);
        Debug.Log("Строительство: Пушка");
    }

    public void StartCrossbowPlacement()
    {
        StartPlacing(weaponCrossbowPrefab);
        Debug.Log("Строительство: Арбалет");
    }

    // Универсальный метод запуска размещения
    private void StartPlacing(GameObject prefab)
    {
        isPlacing = true;
        currentRotation = Quaternion.identity;
        currentPrefab = prefab;

        if (previewObject != null)
            Destroy(previewObject);

        previewObject = Instantiate(prefab);
        previewObject.GetComponent<Collider>().enabled = false;
        SetTransparent(previewObject, true);
    }

    private void Update()
    {
        if (!isPlacing || currentPrefab == null)
            return;

        Vector3Int cell = GetMouseCell();
        if (cell != Vector3Int.one * int.MinValue)
        {
            float yOffset = currentPrefab.transform.position.y;
            Vector3 worldPos = new Vector3(cell.x, yOffset, cell.z);
            previewObject.transform.position = worldPos;
            previewObject.transform.rotation = currentRotation;

            if (Input.GetMouseButtonDown(1)) // ПКМ
            {
                currentRotation *= Quaternion.Euler(0, 90, 0);
                Debug.Log("Поворот на 90 градусов");
            }

            if (Input.GetMouseButtonDown(0)) // ЛКМ
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                lastPlacedObject = Instantiate(currentPrefab, worldPos, currentRotation);
                isPlacing = false;
                Destroy(previewObject);
                previewObject = null;
                currentPrefab = null;
                Debug.Log("Объект размещён");
            }
        }
    }

    // Получение клетки под курсором
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

    // Прозрачность призрака
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

    // Отмена строительства (по кнопке)
    public void CancelPlacement()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }

        currentPrefab = null;
        isPlacing = false;
        Debug.Log("Строительство отменено.");
    }

    // Откат последнего размещения
    public void UndoLastPlacement()
    {
        if (lastPlacedObject != null)
        {
            Destroy(lastPlacedObject);
            lastPlacedObject = null;
            Debug.Log("Последнее размещение отменено.");
        }
    }
}
