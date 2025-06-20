using UnityEngine;
using System;

public class Headquarter : BaseBuilding
{
    public enum UnitType { Worker }

    [Header("UI Menu")]
    public GameObject menuUI;

    [Header("Spawn Settings")]
    public GameObject workerPrefab;
    public Vector2 spawnOffset = new Vector2(3f, 0f);

    public static event Action OnHQDestroyed;

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
            case UnitType.Worker:
                prefab = workerPrefab;
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
        Debug.Log("Headquarter: OnBuildingClicked dipanggil");
        SetActiveBuildingUI(menuUI); // Aktifkan UI ketika dipilih
    }

    public void StoreGold(int amount)
    {
        if (amount <= 0) return;
        
        Debug.Log($"💰 HQ menerima {amount} gold dari Worker.");
        GameManager.Instance.AddGold(amount);
    }

}
