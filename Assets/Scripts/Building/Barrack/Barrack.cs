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

    // Fungsi ini bisa dipanggil dari UI Button (dengan int di Inspector)
    public void SpawnUnit(int unitTypeInt)
    {
        UnitType type = (UnitType)unitTypeInt;
        SpawnUnit(type);
    }

    // Fungsi internal menggunakan enum
    private void SpawnUnit(UnitType type)
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

    public override void Die()
    {
        base.Die();
        if (menuUI != null && menuUI.activeSelf)
            menuUI.SetActive(false);
    }

    protected override void OnBuildingClicked()
    {
        SetActiveBuildingUI(menuUI); // Aktifkan UI ketika dipilih
    }

}
