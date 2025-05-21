using UnityEngine;

public class Character2DClickMove : MonoBehaviour
{
    private static Character2DClickMove selectedCharacter;
    private SpriteRenderer spriteRenderer;
    public Color highlightColor = Color.yellow;
    private Color originalColor;

    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving = false;

    public Transform boundMin; // GameObject batas kiri bawah
    public Transform boundMax; // GameObject batas kanan atas

    private Vector2 minBounds;
    private Vector2 maxBounds;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        targetPosition = transform.position;

        // Ambil nilai posisi dari GameObject batas
        if (boundMin != null && boundMax != null)
        {
            minBounds = boundMin.position;
            maxBounds = boundMax.position;
        }
        else
        {
            Debug.LogWarning("BoundMin dan BoundMax belum diset!");
        }
    }

    void OnMouseDown()
    {
        if (selectedCharacter != null && selectedCharacter != this)
        {
            selectedCharacter.spriteRenderer.color = selectedCharacter.originalColor;
        }

        selectedCharacter = this;
        spriteRenderer.color = highlightColor;
    }

    void Update()
    {
        if (selectedCharacter == this)
        {
            if (Input.GetMouseButtonDown(1)) // klik kanan
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0f;

                // Clamp target dengan batas map
                mouseWorldPos.x = Mathf.Clamp(mouseWorldPos.x, minBounds.x, maxBounds.x);
                mouseWorldPos.y = Mathf.Clamp(mouseWorldPos.y, minBounds.y, maxBounds.y);

                targetPosition = mouseWorldPos;
                isMoving = true;
            }

            if (isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
                {
                    isMoving = false;
                }
            }
        }
    }

    void OnMouseEnter()
    {
        if (selectedCharacter != this)
        {
            spriteRenderer.color = highlightColor;
        }
    }

    void OnMouseExit()
    {
        if (selectedCharacter != this)
        {
            spriteRenderer.color = originalColor;
        }
    }
}
