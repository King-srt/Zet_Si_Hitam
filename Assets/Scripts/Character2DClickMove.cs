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

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        targetPosition = transform.position;
    }

    void OnMouseDown()
    {
        // Pilih karakter ini
        if (selectedCharacter != null && selectedCharacter != this)
        {
            selectedCharacter.spriteRenderer.color = selectedCharacter.originalColor;
        }

        selectedCharacter = this;
        spriteRenderer.color = highlightColor;
    }

    void Update()
    {
        // === Arahkan ke tempat klik kanan jika karakter ini sedang dipilih ===
        if (selectedCharacter == this && Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            targetPosition = mouseWorldPos;
            isMoving = true;
        }

        // === Karakter tetap bergerak ke target meski tidak dipilih ===
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                isMoving = false;
            }
        }

        // === Klik kiri di luar karakter untuk membatalkan pilihan ===
        if (selectedCharacter == this && Input.GetMouseButtonDown(0))
        {
            // Raycast untuk memastikan tidak klik karakter
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider == null || hit.collider.gameObject != gameObject)
            {
                spriteRenderer.color = originalColor;
                selectedCharacter = null;
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
