using UnityEngine;

public class BarrackSpawner : MonoBehaviour
{
    public GameObject unitPrefab;         // prefab unit yang ingin di-spawn
    public Vector2 offset = new Vector2(1f, 0f); // jarak relatif dari gedung (misalnya ke kanan)

    void OnMouseDown()
    {
        Vector2 spawnPosition = (Vector2)transform.position + offset;
        Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
    }
}
