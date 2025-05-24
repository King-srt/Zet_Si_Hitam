using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Character2DClickMove : MonoBehaviour
{
    private static Character2DClickMove selectedCharacter;
    private SpriteRenderer spriteRenderer;
    public Color highlightColor = Color.yellow;
    private Color originalColor;

    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving = false;

    private Rigidbody2D rb;

    // Event untuk komunikasi dengan kamera
    public delegate void OnCharacterSelected(Character2DClickMove character);
    public static event OnCharacterSelected CharacterSelected;

    public delegate void OnCharacterMoved(Vector3 position);
    public static event OnCharacterMoved CharacterMoved;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        targetPosition = transform.position;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic; // Default: tidak dipilih, jadi kinematic
    }

    void OnMouseDown()
    {
        if (selectedCharacter != null && selectedCharacter != this)
        {
            selectedCharacter.spriteRenderer.color = selectedCharacter.originalColor;
            selectedCharacter.SetKinematic(true); // Jadikan karakter sebelumnya kinematic
        }

        selectedCharacter = this;
        spriteRenderer.color = highlightColor;
        SetKinematic(false); // Ubah ke dynamic saat dipilih

        // Kirim event karakter dipilih ke kamera
        CharacterSelected?.Invoke(this);
    }

    void Update()
    {
        if (selectedCharacter == this && Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            targetPosition = mouseWorldPos;
            isMoving = true;
        }

        if (selectedCharacter == this && Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            Collider2D hit = Physics2D.OverlapPoint(mousePos2D);
            if (hit == null || hit.gameObject != gameObject)
            {
                spriteRenderer.color = originalColor;
                SetKinematic(true); // Saat tidak dipilih, kembali ke kinematic
                selectedCharacter = null;
            }
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 nextPos = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(nextPos);

            // Kirim posisi bergerak ke kamera
            CharacterMoved?.Invoke(transform.position);

            if (Vector2.Distance(rb.position, targetPosition) < 0.05f)
            {
                isMoving = false;
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

    private void SetKinematic(bool isKinematic)
    {
        rb.bodyType = isKinematic ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
    }
}
