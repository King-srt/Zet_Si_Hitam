using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject zombiePrefab;
    public int zombiesPerWave = 5;
    public float spawnRadius = 5f;
    public float spawnInterval = 1f;

    [Header("Runtime")]
    public List<ZombieAI> activeZombies = new List<ZombieAI>();
    public bool spawning = false;

    void OnEnable()
    {
        GameManager.OnTimeChanged += HandleTimeChanged;
    }

    void OnDisable()
    {
        GameManager.OnTimeChanged -= HandleTimeChanged;
    }

    void Start()
    {
        // Remove this: StartCoroutine(SpawnWave());
    }

    private void HandleTimeChanged(bool isNight)
    {
        if (isNight && !spawning)
        {
            StartCoroutine(SpawnWave());
        }
        else if (!isNight)
        {
            // Optionally, stop spawning if day starts
            StopSpawning();
        }
    }

    public IEnumerator SpawnWave()
    {
        spawning = true;

        for (int i = 0; i < zombiesPerWave; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(spawnInterval);
        }

        spawning = false;
    }

    void SpawnZombie()
    {
        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
        GameObject zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);

        ZombieAI zombieAI = zombie.GetComponent<ZombieAI>();
        if (zombieAI != null)
        {
            zombieAI.spawner = this;
            activeZombies.Add(zombieAI);
        }
    }

    public void OnZombieDestroyed(ZombieAI zombie)
    {
        if (activeZombies.Contains(zombie))
        {
            activeZombies.Remove(zombie);
        }
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
        spawning = false;
    }

    public void ForceRetreatAll()
    {
        foreach (var zombie in activeZombies)
        {
            if (zombie != null)
                zombie.StartRetreat();
        }
    }
}
