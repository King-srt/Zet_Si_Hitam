using UnityEngine;

public class Barrack : BaseBuilding
{
    public enum UnitType { Knight, Archer }

    [Header("UI Menu")]
    public GameObject menuUI;

    [Header("Spawn Settings")]
    public GameObject knightPrefab;
    public GameObject archerPrefab;
    public Vector2 spawnOffset = new Vector2(3f, 0f);

    // Fungsi yang dipanggil dari UI (gunakan int di Inspector button)
    public void SpawnUnit(int unitTypeInt)
    {
        UnitType type = (UnitType)unitTypeInt;
        int cost = GetUnitCost(type);

        if (GameManager.Instance.GetGold() >= cost)
        {
            GameManager.Instance.SpendGold(cost);
            SpawnUnit_Internal(type);
            AddUnitCount(type);
        }
        else
        {
            Debug.LogWarning($"❌ Gold tidak cukup untuk memanggil {type}! (Butuh: {cost}, Punya: {GameManager.Instance.GetGold()})");
        }
    }

    private void SpawnUnit_Internal(UnitType type)
    {
        GameObject prefab = null;

        switch (type)
        {
            case UnitType.Knight:
                prefab = knightPrefab;
                break;
            case UnitType.Archer:
                prefab = archerPrefab;
                break;
        }

        if (prefab != null)
        {
            Vector2 spawnPosition = (Vector2)transform.position + spawnOffset;
            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"❌ Prefab untuk {type} belum di-assign di Inspector.");
        }
    }

    private int GetUnitCost(UnitType type)
    {
        switch (type)
        {
            case UnitType.Knight:
                return 20;
            case UnitType.Archer:
                return 25;
            default:
                return 0;
        }
    }

    private void AddUnitCount(UnitType type)
    {
        switch (type)
        {
            case UnitType.Knight:
                GameManager.Instance.AddKnight();
                break;
            case UnitType.Archer:
                GameManager.Instance.AddArcher();
                break;
        }
    }

    public override void Die()
    {
        base.Die();
        if (menuUI != null && menuUI.activeSelf)
            menuUI.SetActive(false);
    }

    protected override void OnBuildingClicked()
    {
        SetActiveBuildingUI(menuUI);
    }
}
