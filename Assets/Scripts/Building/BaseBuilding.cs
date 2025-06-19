using UnityEngine;

public class BaseBuilding : MonoBehaviour
{
    // === HEALTH SETTINGS ===
    [Header("Health Settings")]
    public int maxHP = 100;
    private int currentHP;

    [Header("Optional")]
    public bool isCritical = false;

    private static BaseBuilding selectedBuilding;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    // === EVENTS ===
    public delegate void OnBuildingSelected(BaseBuilding building);
    public static event OnBuildingSelected BuildingSelected;

    // === INIT ===
    protected virtual void Start()
    {
        currentHP = maxHP;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // === INPUT SELEKSI SAJA ===
    void OnMouseDown()
    {
        SelectBuilding();
    }


    void Update()
    {
        if (selectedBuilding == this && Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            Collider2D hit = Physics2D.OverlapPoint(mousePos2D);
            if (hit == null || hit.gameObject != gameObject)
            {
                spriteRenderer.color = originalColor;
                selectedBuilding = null;
            }
        }
    }

    // === HEALTH & DAMAGE ===
    public void TakeDamage(int amount)
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

    // Tambahkan method ini di dalam BaseBuilding
    protected void SelectBuilding()
    {
        if (selectedBuilding != null && selectedBuilding != this)
        {
            selectedBuilding.spriteRenderer.color = selectedBuilding.originalColor;
        }

        selectedBuilding = this;
        spriteRenderer.color = Color.yellow;

        BuildingSelected?.Invoke(this);
    }

}
