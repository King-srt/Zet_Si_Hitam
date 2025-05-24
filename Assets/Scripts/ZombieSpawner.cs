using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform baseTransform;
    public float spawnRadius = 8f;
    public float spawnInterval = 5f;
    public int maxZombies = 10;

    private int currentZombies = 0;

    void Start()
    {
        InvokeRepeating(nameof(SpawnZombie), 1f, spawnInterval);
    }

    void SpawnZombie()
    {
        if (currentZombies >= maxZombies) return;

        if (baseTransform == null)
        {
            Debug.LogWarning("Base transform sudah dihancurkan. Zombie tidak bisa spawn.");
            CancelInvoke(nameof(SpawnZombie));
            return;
        }

        Vector2 spawnPos = (Vector2)baseTransform.position + Random.insideUnitCircle.normalized * spawnRadius;
        GameObject zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);

        ZombieAI ai = zombie.GetComponent<ZombieAI>();
        if (ai != null)
        {
            ai.targetBase = baseTransform;
            ai.spawner = this;
        }

        currentZombies++;
    }

    // ✅ Versi yang menerima parameter ZombieAI
    public void OnZombieDestroyed(ZombieAI zombie)
    {
        currentZombies = Mathf.Max(0, currentZombies - 1);
        Debug.Log($"Zombie {zombie.name} dihancurkan. Sisa zombie: {currentZombies}");
    }

    public void StopSpawning()
    {
        CancelInvoke(nameof(SpawnZombie));
        Debug.Log("Spawner dimatikan karena semua target telah hancur.");
    }
}
