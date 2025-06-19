using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BaseUnit : MonoBehaviour
{
    public static BaseUnit selectedUnit;

    // === HEALTH SETTINGS ===
    [Header("Health Settings")]
    [SerializeField] protected int maxHP = 100;
    protected int currentHP;
    [Header("Critical Unit?")]
    [SerializeField] protected bool isCritical = false;

    // === MOVEMENT SETTINGS ===
    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 5f;

    // === STATES ===
    [SerializeField] protected Color highlightColor = Color.yellow;
    private Vector3 targetPosition;
    protected bool isMoving = false;

    // === COMPONENTS ===
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected Color originalColor;

    // === EVENTS ===
    public delegate void OnUnitSelected(BaseUnit unit);
    public static event OnUnitSelected UnitSelected;

    public delegate void OnUnitMoved(Vector3 position);
    public static event OnUnitMoved UnitMoved;

    // === INIT ===
    protected virtual void Start()
    {
        currentHP = maxHP;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        originalColor = spriteRenderer.color;
        targetPosition = transform.position;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic; // Always keep as Kinematic for MovePosition
    }

    // === INPUT SELEKSI DAN GERAK ===
    protected virtual void OnMouseDown()
    {
        if (selectedUnit != null && selectedUnit != this)
        {
            selectedUnit.spriteRenderer.color = selectedUnit.originalColor;
        }

        selectedUnit = this;
        spriteRenderer.color = highlightColor;

        UnitSelected?.Invoke(this);
    }

    protected virtual void Update()
    {
        if (selectedUnit == this && Input.GetMouseButtonDown(1))
{
    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mouseWorldPos.z = 0f;

    // Cek apakah klik kanan diarahkan ke Tower
    Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
    Collider2D hit = Physics2D.OverlapPoint(mousePos2D);

            if (hit != null && hit.TryGetComponent(out Tower tower))
            {
                // Hanya izinkan jika unit ini adalah Archer
                if (this is Archer archer)
                {
                    if (tower.TryInsertArcher(archer))
                    {
                        Debug.Log("Archer berhasil naik ke tower.");
                        return;
                    }
                    else
                    {
                        Debug.Log("Tower sudah berisi Archer.");
                        return;
                    }
                }
                else
                {
                    Debug.Log("Unit ini tidak dapat naik ke tower.");
                    return;
                }
            }

            // Jika bukan klik ke Tower, maka lanjutkan movement biasa
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

    public void SetTargetPosition(Vector3 position)
    {
    Vector3 scale = transform.localScale;
    scale.x = Mathf.Abs(scale.x) * Mathf.Sign(position.x - transform.position.x);
    transform.localScale = scale;
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
        Debug.Log($"{gameObject.name} mati! Menunggu subclass untuk handle Destroy.");
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