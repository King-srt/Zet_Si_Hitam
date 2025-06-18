using UnityEngine;
using System;

public class Headquarter : BaseBuilding
{
    [Header("UI Menu")]
    public GameObject menuUI;

    [Header("Spawn Settings")]
    public GameObject unitPrefab;
    public Vector2 spawnOffset = new Vector2(1f, 0f);

    public static event Action OnHQDestroyed;

    // Ketika bangunan diklik
    void OnMouseDown()
    {
    // Panggil logika seleksi dari base class
        SelectBuilding();

    // Toggle menu UI
        if (menuUI != null)
        {
            if (menuUI.activeSelf)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
    }


    public void OpenMenu()
    {
        menuUI.SetActive(true);
        menuUI.transform.position = transform.position + new Vector3(0, -1.5f, 0);
    }

    public void CloseMenu()
    {
        menuUI.SetActive(false);
    }

    public void SpawnUnit()
    {
        if (unitPrefab != null)
        {
            Vector2 spawnPosition = (Vector2)transform.position + spawnOffset;
            Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public override void Die()
    {
        if (IsDead()) return;

        base.Die();

        Debug.Log("HQ Destroyed!");
        OnHQDestroyed?.Invoke(); // Panggil event untuk GameManager
    }
}
