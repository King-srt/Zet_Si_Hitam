using UnityEngine;
using UnityEngine.EventSystems;

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

    public static GameObject activeBuildingUI = null;

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

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Cek apakah klik di UI (jangan proses klik bangunan kalau iya)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // Ambil posisi klik
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            // Deteksi apakah ada collider di titik klik
            Collider2D hit = Physics2D.OverlapPoint(mousePos2D);

            // === Klik ke bangunan ini ===
            if (hit != null && hit.gameObject == gameObject)
            {
                SelectBuilding();
                OnBuildingClicked(); // ← subclass override UI aktifkan
            }
            // === Klik ke tempat lain, reset seleksi dan UI ===
            else if (selectedBuilding == this)
            {
                if (spriteRenderer != null)
                    spriteRenderer.color = originalColor;

                selectedBuilding = null;

                if (activeBuildingUI != null)
                {
                    activeBuildingUI.SetActive(false);
                    activeBuildingUI = null;
                }
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

    //Kontrol UI Global
    protected void SetActiveBuildingUI(GameObject newUI)
    {
        // Tutup UI lama kalau ada
        if (activeBuildingUI != null && activeBuildingUI != newUI)
        {
            activeBuildingUI.SetActive(false);
        }

        // Toggle UI baru
        if (newUI != null)
        {
            bool willBeActive = !newUI.activeSelf;
            newUI.SetActive(willBeActive);

            if (willBeActive)
            {
                newUI.transform.position = transform.position + new Vector3(0, -1.5f, 0);
                activeBuildingUI = newUI;
            }
            else
            {
                activeBuildingUI = null;
            }
        }
    }

    protected virtual void OnEnable()
    {
        BaseUnit.UnitSelected += HandleUnitSelected;
    }

    protected virtual void OnDisable()
    {
        BaseUnit.UnitSelected -= HandleUnitSelected;
    }

    private void HandleUnitSelected(BaseUnit unit)
    {
        // Saat unit dipilih, sembunyikan UI bangunan aktif
        if (activeBuildingUI != null)
        {
            activeBuildingUI.SetActive(false);
            activeBuildingUI = null;
        }
    }

    protected virtual void OnBuildingClicked()
    {
        // Dibiarkan kosong, akan di-override di subclass
    }

}
