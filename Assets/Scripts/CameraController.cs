using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public Vector2 panLimit = new Vector2(50f, 50f);
    public float scrollSpeed = 20f;
    public float minY = 5f;
    public float maxY = 120f;
    public float rotationSpeed = 50f;
    
    public float minVerticalAngle = 10f;
    public float maxVerticalAngle = 80f;
    private float verticalAngle = 45f; 

    private Transform camTransform;

    void Start()
    {
        camTransform = GetComponentInChildren<Camera>().transform;
        camTransform.localRotation = Quaternion.Euler(verticalAngle, 0f, 0f);
    }

    void Update()
    {
        Vector3 pos = transform.position;
        
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        if (Input.GetKey("w"))
        {
            pos += forward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            pos -= forward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            pos += right * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            pos -= right * panSpeed * Time.deltaTime;
        }
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime, Space.World);
            
            float mouseY = Input.GetAxis("Mouse Y");
            verticalAngle -= mouseY * rotationSpeed * Time.deltaTime;
            verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
            camTransform.localRotation = Quaternion.Euler(verticalAngle, 0f, 0f);
        }
    }
}
