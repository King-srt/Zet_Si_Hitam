using UnityEngine;

public class SoldierSpawner : MonoBehaviour
{
    public GameObject soldierPrefab;
    public Vector2 offset = new Vector2(1f, 0f); // posisi spawn relatif dari barrack

    public void SpawnSoldier()
    {
        Vector2 spawnPos = (Vector2)transform.position + offset;
        Instantiate(soldierPrefab, spawnPos, Quaternion.identity);
    }
}
