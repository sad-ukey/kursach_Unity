using UnityEngine;

public class BuildingDrag : MonoBehaviour
{
    public GridManager gridManager; // Ссылка на GridManager для работы с сеткой
    private Camera mainCamera;
    
    // Здесь можно добавить ссылку на объект подсветки ячейки, если потребуется визуальная индикация
    // private GameObject cellHighlight;
    
    void Start()
    {
        mainCamera = Camera.main;
        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();
    }
    
    void Update()
    {
        // Создаем плоскость на уровне gridOrigin.y
        Plane gridPlane = new Plane(Vector3.up, gridManager.gridOrigin);
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float distance;
        
        if (gridPlane.Raycast(ray, out distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            // Определяем, в какой ячейке находится курсор
            Vector2Int gridPos = gridManager.GetGridPosition(worldPoint);
            // Находим центр ячейки
            Vector3 snappedPosition = gridManager.GetWorldPosition(gridPos);
            transform.position = snappedPosition;
            
            // Здесь можно обновить подсветку ячейки, если реализуешь визуальный эффект
            // UpdateCellHighlight(snappedPosition);
        }
        
        // При нажатии левой кнопки мыши фиксируем позицию постройки
        if (Input.GetMouseButtonDown(0))
        {
            // Проверяем, свободна ли ячейка (на данном этапе всегда true)
            if (IsCellFree())
            {
                Destroy(this); // Выключаем режим перетаскивания
                RemoveCellHighlight();
            }
            else
            {
                Debug.Log("Ячейка занята! Выберите другое место.");
            }
        }
    }
    
    // Псевдопроверка – всегда возвращает true. Здесь можно добавить проверку через Physics.OverlapBox или ведение списка занятых ячеек.
    private bool IsCellFree()
    {
        return true;
    }
    
    private void RemoveCellHighlight()
    {
        // Если реализуешь подсветку – удаляй её здесь
    }
    
    // Метод для обновления подсветки ячейки можно добавить здесь (опционально)
    // private void UpdateCellHighlight(Vector3 position) { ... }
}
