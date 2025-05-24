using UnityEngine;
using System.Collections;

public class CameraFollowSelectedCharacter : MonoBehaviour
{
    public float followSpeed = 5f;
    public float smoothMoveDuration = 0.5f; // durasi smooth move ke karakter

    private Transform target;
    private Vector3 targetPosition;

    private bool hasDragged = false;
    private Vector3 lastMousePosition;

    private Coroutine smoothMoveCoroutine;

    void OnEnable()
    {
        Character2DClickMove.CharacterSelected += OnCharacterSelected;
        Character2DClickMove.CharacterMoved += OnCharacterMoved;
    }

    void OnDisable()
    {
        Character2DClickMove.CharacterSelected -= OnCharacterSelected;
        Character2DClickMove.CharacterMoved -= OnCharacterMoved;
    }

    void OnCharacterSelected(Character2DClickMove character)
    {
        target = character.transform;
        targetPosition = target.position;

        hasDragged = false;

        // Jika ada coroutine smooth move yang berjalan, hentikan dulu
        if (smoothMoveCoroutine != null)
            StopCoroutine(smoothMoveCoroutine);

        // Mulai smooth move kamera ke posisi karakter
        smoothMoveCoroutine = StartCoroutine(SmoothMoveCamera(new Vector3(targetPosition.x, targetPosition.y, transform.position.z)));
    }

    IEnumerator SmoothMoveCamera(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < smoothMoveDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / smoothMoveDuration);
            yield return null;
        }

        transform.position = targetPos;
        smoothMoveCoroutine = null;
    }

    void OnCharacterMoved(Vector3 pos)
    {
        if (target != null && target.position == pos)
        {
            targetPosition = pos;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            if (mouseDelta.sqrMagnitude > 0.01f)
            {
                hasDragged = true;
                float dragSpeed = 0.01f;

                Vector3 dragMove = new Vector3(-mouseDelta.x, -mouseDelta.y, 0) * dragSpeed;

                Camera cam = Camera.main;
                if (cam != null)
                {
                    dragMove *= cam.orthographicSize / 5f;
                }

                transform.position += dragMove;
                lastMousePosition = Input.mousePosition;

                // Kalau user drag, hentikan smooth move kalau sedang berjalan
                if (smoothMoveCoroutine != null)
                {
                    StopCoroutine(smoothMoveCoroutine);
                    smoothMoveCoroutine = null;
                }
            }
        }

        Vector3 keyboardMove = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
        if (keyboardMove.sqrMagnitude > 0.01f)
        {
            hasDragged = true;
            float keyboardSpeed = 10f;
            transform.position += keyboardMove.normalized * keyboardSpeed * Time.deltaTime;

            if (smoothMoveCoroutine != null)
            {
                StopCoroutine(smoothMoveCoroutine);
                smoothMoveCoroutine = null;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (!hasDragged && smoothMoveCoroutine == null)
        {
            Vector3 desiredPosition = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }
    }
}
