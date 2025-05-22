using UnityEngine;

public class CameraEdgeScroll : MonoBehaviour
{
    public float scrollSpeed = 10f;
    public float edgeSize = 10f; // Ukuran tepi dalam pixel

    public Transform mapBoundMin; // Drag GameObject MapBoundMin ke sini
    public Transform mapBoundMax; // Drag GameObject MapBoundMax ke sini

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (mapBoundMin == null || mapBoundMax == null) return;

        Vector3 pos = transform.position;
        Vector3 mousePos = Input.mousePosition;

        // Deteksi posisi mouse terhadap tepi layar
        if (mousePos.x >= Screen.width - edgeSize)
            pos.x += scrollSpeed * Time.deltaTime;
        else if (mousePos.x <= edgeSize)
            pos.x -= scrollSpeed * Time.deltaTime;

        if (mousePos.y >= Screen.height - edgeSize)
            pos.y += scrollSpeed * Time.deltaTime;
        else if (mousePos.y <= edgeSize)
            pos.y -= scrollSpeed * Time.deltaTime;

        // Batasi kamera agar tidak keluar dari bounds
        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;

        Vector2 minBounds = mapBoundMin.position;
        Vector2 maxBounds = mapBoundMax.position;

        pos.x = Mathf.Clamp(pos.x, minBounds.x + camHalfWidth, maxBounds.x - camHalfWidth);
        pos.y = Mathf.Clamp(pos.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);

        transform.position = pos;
    }
}
