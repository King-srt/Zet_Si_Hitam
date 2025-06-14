using UnityEngine;

public class CameraFocusedFollowingUnit : MonoBehaviour
{
    public Transform target; // objek yang diikuti kamera (opsional)
    public float moveSpeed = 5f;

    public Transform boundMin; // GameObject batas kiri bawah
    public Transform boundMax; // GameObject batas kanan atas

    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    private float camHalfHeight;
    private float camHalfWidth;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        UpdateCameraSize();
    }

    void LateUpdate()
    {
        // === Zoom kamera dengan scroll ===
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
            UpdateCameraSize();
        }

        Vector3 targetPos;

        if (target != null)
        {
            // Kamera mengikuti target
            targetPos = Vector3.Lerp(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            // Kamera bisa dikontrol manual jika tidak mengikuti target
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            targetPos = transform.position + new Vector3(h, v, 0) * moveSpeed * Time.deltaTime;
        }

        // Clamp agar kamera tidak keluar dari batas map
        float clampedX = Mathf.Clamp(targetPos.x, boundMin.position.x + camHalfWidth, boundMax.position.x - camHalfWidth);
        float clampedY = Mathf.Clamp(targetPos.y, boundMin.position.y + camHalfHeight, boundMax.position.y - camHalfHeight);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    void UpdateCameraSize()
    {
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }
}
