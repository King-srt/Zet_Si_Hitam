using UnityEngine;

public class WorkerSpawner : MonoBehaviour
{
    public GameObject workerPrefab;
    public Vector2 offset = new Vector2(1f, 0f); // posisi spawn relatif dari barrack

    public void SpawnWorker()
    {
        Vector2 spawnPos = (Vector2)transform.position + offset;
        Instantiate(workerPrefab, spawnPos, Quaternion.identity);
    }
}
