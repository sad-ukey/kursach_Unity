using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit = new Vector2(50f, 50f);
    public float scrollSpeed = 20f;
    public float minY = 20f;
    public float maxY = 120f;
    public float rotationSpeed = 50f;
    
    // Ограничения для вертикального вращения камеры (наклона)
    public float minVerticalAngle = 10f;
    public float maxVerticalAngle = 80f;
    private float verticalAngle = 45f; // начальный угол наклона

    private Transform camTransform;

    void Start()
    {
        // Предполагаем, что у CameraRig есть дочерняя камера
        camTransform = GetComponentInChildren<Camera>().transform;
        camTransform.localRotation = Quaternion.Euler(verticalAngle, 0f, 0f);
    }

    void Update()
    {
        Vector3 pos = transform.position;
        
        // Рассчитываем горизонтальные векторы (без учёта вертикали)
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        // Перемещение: клавиши WASD или курсор у края экрана (работают, если правая кнопка не зажата)
        if (Input.GetKey("w") || (!Input.GetMouseButton(1) && Input.mousePosition.y >= Screen.height - panBorderThickness))
        {
            pos += forward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || (!Input.GetMouseButton(1) && Input.mousePosition.y <= panBorderThickness))
        {
            pos -= forward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || (!Input.GetMouseButton(1) && Input.mousePosition.x >= Screen.width - panBorderThickness))
        {
            pos += right * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || (!Input.GetMouseButton(1) && Input.mousePosition.x <= panBorderThickness))
        {
            pos -= right * panSpeed * Time.deltaTime;
        }
        
        // Зумирование с помощью колёсика мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

        // Ограничиваем позицию по осям
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;

        // Вращение камеры при удержании правой кнопки мыши
        if (Input.GetMouseButton(1))
        {
            // Горизонтальное вращение (поворот CameraRig вокруг вертикальной оси)
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime, Space.World);
            
            // Вертикальное вращение (наклон камеры вверх/вниз)
            float mouseY = Input.GetAxis("Mouse Y");
            verticalAngle -= mouseY * rotationSpeed * Time.deltaTime;
            verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
            camTransform.localRotation = Quaternion.Euler(verticalAngle, 0f, 0f);
        }
    }
}
