using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    // === HEALTH SETTINGS ===
    [Header("Health Settings")]
    public int maxHP = 100;
    private int currentHP;

    [Header("Optional")]
    public bool isCritical = false;

    // === MOVEMENT SETTINGS ===
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Color highlightColor = Color.yellow;

    private static BaseUnit selectedUnit;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private Rigidbody2D rb;

    // === EVENTS ===
    public delegate void OnUnitSelected(BaseUnit unit);
    public static event OnUnitSelected UnitSelected;

    public delegate void OnUnitMoved(Vector3 position);
    public static event OnUnitMoved UnitMoved;

    // === INIT ===
    protected virtual void Start()
    {
        currentHP = maxHP;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        targetPosition = transform.position;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    // === INPUT SELEKSI DAN GERAK ===
    void OnMouseDown()
    {
        if (selectedUnit != null && selectedUnit != this)
        {
            selectedUnit.spriteRenderer.color = selectedUnit.originalColor;
            selectedUnit.SetKinematic(true);
        }

        selectedUnit = this;
        spriteRenderer.color = highlightColor;
        SetKinematic(false);

        UnitSelected?.Invoke(this);
    }

    protected virtual void Update()
    {
        if (selectedUnit == this && Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            SetTargetPosition(mouseWorldPos);
        }

        if (selectedUnit == this && Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            Collider2D hit = Physics2D.OverlapPoint(mousePos2D);
            if (hit == null || hit.gameObject != gameObject)
            {
                spriteRenderer.color = originalColor;
                SetKinematic(true);
                selectedUnit = null;
            }
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 nextPos = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(nextPos);

            UnitMoved?.Invoke(transform.position);

            if (Vector2.Distance(rb.position, targetPosition) < 0.05f)
            {
                isMoving = false;
            }
        }
    }

    private void SetKinematic(bool isKinematic)
    {
        rb.bodyType = isKinematic ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
        isMoving = true;
    }

    // === HEALTH & DAMAGE ===
    public virtual void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed!");
        Destroy(gameObject);
    }

    // === HEALTH UTIL ===
    public bool IsDead()
    {
        return currentHP <= 0;
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public float GetHealthPercent()
    {
        return (float)currentHP / maxHP;
    }
}
