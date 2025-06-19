using UnityEngine;
using UnityEngine.UI;

public class Tower : BaseBuilding
{
    [Header("Tower Settings")]
    public Transform archerPoint; // Posisi archer di dalam tower
    public GameObject towerUI; // Panel UI saat tower dipilih
    public Button exitButton;
    public Text statusText;

    private Archer storedArcher;

    protected override void Start()
    {
        base.Start();

        if (towerUI != null)
            towerUI.SetActive(false);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitButtonPressed);
    }

    void OnMouseDown()
    {
        SelectBuilding();

        if (towerUI != null)
        {
            towerUI.SetActive(true);
            UpdateUI();
        }
    }

    public bool HasArcher()
    {
        return storedArcher != null;
    }

    public bool TryInsertArcher(Archer archer)
    {
        if (storedArcher != null || archer == null)
            return false;

        storedArcher = archer;
        archer.EnterTower(this);
        UpdateUI();
        return true;
    }

    private void OnExitButtonPressed()
    {
        if (storedArcher != null)
        {
            storedArcher.ExitTower();
            storedArcher = null;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (storedArcher != null)
        {
            statusText.text = "Archer ditempatkan";
            exitButton.interactable = true;
        }
        else
        {
            statusText.text = "Archer tidak tersedia";
            exitButton.interactable = false;
        }
    }

    public Transform GetArcherPoint()
    {
        return archerPoint;
    }

}
