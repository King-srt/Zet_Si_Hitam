using UnityEngine;

public class CameraDragScroll : MonoBehaviour
{
    public float dragSpeed = 0.5f;
    public Transform mapBoundMin;
    public Transform mapBoundMax;

    private Camera cam;
    private Vector3 dragOrigin;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (mapBoundMin == null || mapBoundMax == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPos = transform.position + difference * dragSpeed;

            // Clamp posisi kamera
            float camHalfHeight = cam.orthographicSize;
            float camHalfWidth = camHalfHeight * cam.aspect;

            float clampedX = Mathf.Clamp(newPos.x, mapBoundMin.position.x + camHalfWidth, mapBoundMax.position.x - camHalfWidth);
            float clampedY = Mathf.Clamp(newPos.y, mapBoundMin.position.y + camHalfHeight, mapBoundMax.position.y - camHalfHeight);

            transform.position = new Vector3(clampedX, clampedY, transform.position.z);

            // Update drag origin untuk smooth dragging
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
