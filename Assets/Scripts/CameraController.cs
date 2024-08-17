using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 10f;
    public float dragSpeed = 2f;
    
    public float maxZoom = 20f;
    public float minZoom = 2f;
    
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -10f;
    public float maxY = 10f;
    
    private Camera cam;

    private Vector3 dragOrigin;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleZoom();
        HandleDrag();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            var orthographicSize = cam.orthographicSize;
            orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = orthographicSize;
            cam.orthographicSize = Mathf.Clamp(orthographicSize, minZoom, maxZoom);
        }
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            
            Vector3 position = cam.transform.position;
            position += difference;

            // Clamp the camera position within the specified range
            position = new Vector3(
                Mathf.Clamp(position.x, minX, maxX),
                Mathf.Clamp(position.y, minY, maxY),
                position.z
            );
            cam.transform.position = position;
        }
    }
}