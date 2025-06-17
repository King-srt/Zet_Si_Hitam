using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using UnityEngine.EventSystems;

public class CameraMovements : MonoBehaviour
{
    [Header("Follow Target (Optional)")]
    public Transform target; // Target yang diikuti kamera (bisa null)
    public float followSpeed = 10f;
    public float smoothMoveDuration = 0.3f; // Durasi smooth move ke target

    [Header("Manual Control")]
    public float moveSpeed = 10f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    [Header("Tilemap Clamp")]
    public Tilemap tilemap; // Assign tilemap di Inspector

    private float camHalfHeight, camHalfWidth;
    private Camera cam;
    private Bounds tilemapBounds;
    private bool hasDragged = false;
    private Vector3 lastMousePosition;
    private Coroutine smoothMoveCoroutine;

    void Start()
    {
        cam = Camera.main;
        UpdateCameraSize();
        if (tilemap != null)
            tilemapBounds = tilemap.localBounds;
    }

    void OnEnable()
    {
        // Subscribe ke event unit dipilih/dipindahkan jika ada
        BaseUnit.UnitSelected += OnUnitSelected;
        BaseUnit.UnitMoved += OnUnitMoved;
    }

    void OnDisable()
    {
        BaseUnit.UnitSelected -= OnUnitSelected;
        BaseUnit.UnitMoved -= OnUnitMoved;
    }

    void OnUnitSelected(BaseUnit unit)
    {
        target = unit.transform;
        hasDragged = false;
        // Smooth move ke target
        if (smoothMoveCoroutine != null)
            StopCoroutine(smoothMoveCoroutine);
        smoothMoveCoroutine = StartCoroutine(SmoothMoveCamera(new Vector3(target.position.x, target.position.y, transform.position.z)));
    }

    void OnUnitMoved(Vector3 pos)
    {
        if (target != null)
        {
            // Smooth move ke posisi baru target
            if (smoothMoveCoroutine != null)
                StopCoroutine(smoothMoveCoroutine);
            smoothMoveCoroutine = StartCoroutine(SmoothMoveCamera(new Vector3(pos.x, pos.y, transform.position.z)));
        }
    }

    IEnumerator SmoothMoveCamera(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        while (elapsed < smoothMoveDuration)
        {
            transform.position = Vector3.Lerp(startPos, ClampPosition(targetPos), elapsed / smoothMoveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = ClampPosition(targetPos);
        smoothMoveCoroutine = null;
    }

    void Update()
    {
        if (GameManager.IsPaused)
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
    
        // === Zoom ===
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
            UpdateCameraSize();
        }

        // === Mouse Drag ===
        if (Input.GetMouseButtonDown(0))
            lastMousePosition = Input.mousePosition;
        else if (Input.GetMouseButton(0))
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            if (mouseDelta.sqrMagnitude > 0.01f)
            {
                hasDragged = true;
                float dragSpeed = 0.01f;
                Vector3 dragMove = new Vector3(-mouseDelta.x, -mouseDelta.y, 0) * dragSpeed;
                dragMove *= cam.orthographicSize / 5f;
                transform.position += dragMove;
                lastMousePosition = Input.mousePosition;
                if (smoothMoveCoroutine != null)
                {
                    StopCoroutine(smoothMoveCoroutine);
                    smoothMoveCoroutine = null;
                }
            }
        }

        // === Keyboard Move ===
        Vector3 keyboardMove = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
        if (keyboardMove.sqrMagnitude > 0.01f)
        {
            hasDragged = true;
            transform.position += keyboardMove.normalized * moveSpeed * Time.deltaTime;
            if (smoothMoveCoroutine != null)
            {
                StopCoroutine(smoothMoveCoroutine);
                smoothMoveCoroutine = null;
            }
        }
    }

    void LateUpdate()
    {
        // === Follow Target (jika ada dan belum drag/manual) ===
        if (target != null && !hasDragged && smoothMoveCoroutine == null)
        {
            Vector3 desired = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, ClampPosition(desired), followSpeed * Time.deltaTime);
        }

        // === Clamp ke tilemap ===
        transform.position = ClampPosition(transform.position);
    }

    void UpdateCameraSize()
    {
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    Vector3 ClampPosition(Vector3 pos)
    {
        if (tilemap != null)
        {
            float minX = tilemapBounds.min.x + camHalfWidth;
            float maxX = tilemapBounds.max.x - camHalfWidth;
            float minY = tilemapBounds.min.y + camHalfHeight;
            float maxY = tilemapBounds.max.y - camHalfHeight;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }
        return new Vector3(pos.x, pos.y, transform.position.z);
    }

    // Opsional: panggil ini jika ingin kamera kembali follow target setelah drag/manual
    public void ResetFollow()
    {
        hasDragged = false;
    }
}