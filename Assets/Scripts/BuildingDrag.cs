using UnityEngine;

public class BuildingDrag : MonoBehaviour
{
    [Tooltip("Ссылка на GridManager, можно не указывать — найдём автоматически")]
    public GridManager gridManager;

    private Camera mainCamera;
    private float heightOffset;

    void Start()
    {
        // 1) Получаем MainCamera
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("MainCamera не найдена — проверьте тег у камеры!");

        // 2) Ссылка на GridManager
        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();

        // 3) Половина высоты по коллайдеру
        Collider col = GetComponent<Collider>();
        if (col != null)
            heightOffset = col.bounds.extents.y;
        else
            heightOffset = 0f;
    }

    void Update()
    {
        // 1) Создаем горизонтальную плоскость на уровне gridOrigin.y
        Plane gridPlane = new Plane(Vector3.up, gridManager.gridOrigin);
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // 2) Raycast
        if (!gridPlane.Raycast(ray, out float dist))
            return;

        // 3) Точка попадания
        Vector3 worldPoint = ray.GetPoint(dist);

        // 4) Координаты ячейки
        Vector2Int gridPos = gridManager.GetGridPosition(worldPoint);

        // 5) Запрет выхода за границы
        if (gridPos.x < 0 || gridPos.x >= gridManager.gridWidth ||
            gridPos.y < 0 || gridPos.y >= gridManager.gridHeight)
        {
            // не обновляем позицию
            return;
        }

        // 6) Снаппинг по центру ячейки с учётом высоты
        Vector3 cellCenter = gridManager.GetWorldPosition(gridPos);
        transform.position = new Vector3(
            cellCenter.x,
            gridManager.gridOrigin.y + heightOffset,
            cellCenter.z
        );

        // 7) Фиксация по левому клику
        if (Input.GetMouseButtonDown(0))
        {
            if (IsCellFree(gridPos))
            {
                // Завершаем режим drag&drop
                Destroy(this);
            }
            else
            {
                Debug.Log("Ячейка " + gridPos + " занята!");
            }
        }
    }

    // Пока заглушка: всегда true. Позже можно использовать Physics.OverlapBox и хранить занятые ячейки.
    private bool IsCellFree(Vector2Int gridPos)
    {
        return true;
    }
}
