using UnityEngine;

public class ArcherSpawner : MonoBehaviour
{
    public GameObject archerPrefab;
    public Transform barrackTransform;
    public Vector2 offset = new Vector2(1f, 0f); // posisi spawn relatif dari barrack

    public void SpawnArcher()
    {
        Vector2 spawnPos = (Vector2)barrackTransform.position + offset;
        Instantiate(archerPrefab, spawnPos, Quaternion.identity);
    }
}
