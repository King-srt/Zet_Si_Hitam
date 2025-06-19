using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieSpawner : MonoBehaviour
{

 

    [Header("Spawner Settings")]
    public GameObject zombiePrefab;
    public float spawnRadius = 5f;
    public float spawnInterval = 1f;

    [Header("Boss Settings")]
    public GameObject bossZombiePrefab;
    public bool isBossSpawner = false; // <-- tambahkan ini
    private bool bossSpawned = false;
    private bool bossAlive = false;

    [Header("Runtime")]
    public List<ZombieAI> activeZombies = new List<ZombieAI>();
    public bool spawning = false;

    private int zombiesSpawnedThisNight = 0;
    private int maxZombiesThisNight = 0;
    private bool spawnerDisabled = false;

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
        // Tidak perlu spawn di Start
    }

    private void HandleTimeChanged(bool isNight)
    {
        if (spawnerDisabled) return;

        if (isNight && !spawning)
        {
            int day = FindObjectOfType<GameManager>().GetDayCount();
            switch (day)
            {
                case 1: maxZombiesThisNight = 10; break;
                case 2: maxZombiesThisNight = 15; break;
                case 3: maxZombiesThisNight = 20; break;
                case 4: maxZombiesThisNight = 25; break;
                case 5: maxZombiesThisNight = 30; break;
                default:
                    maxZombiesThisNight = 0;
                    spawnerDisabled = true;
                    return;
            }
            zombiesSpawnedThisNight = 0;

            // Spawn BossZombie hanya di day 5, hanya sekali
            if (day == 5 && !bossSpawned && bossZombiePrefab != null && isBossSpawner)
            {
                SpawnBossZombie();
                bossSpawned = true;
                bossAlive = true;
            }

            StartCoroutine(SpawnWave());
        }
        else if (!isNight)
        {
            StopSpawning();
        }
    }

    public IEnumerator SpawnWave()
    {
        spawning = true;

        while (zombiesSpawnedThisNight < maxZombiesThisNight)
        {
            // Cek apakah masih ada target (BaseUnit atau BaseBuilding yang belum mati)
            bool hasTarget = false;
            foreach (string tag in new[] { "Headquarter", "Worker", "Barrack", "Knight", "Archer", "Tower", "Wall" })
            {
                GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject obj in objs)
                {
                    if (obj == null) continue;
                    var unit = obj.GetComponent<BaseUnit>();
                    var building = obj.GetComponent<BaseBuilding>();
                    if ((unit != null && !unit.IsDead()) || (building != null && !building.IsDead()))
                    {
                        hasTarget = true;
                        break;
                    }
                }
                if (hasTarget) break;
            }

            if (!hasTarget)
            {
                StopSpawning();
                yield break;
            }

            // Pada hari ke-5, jika boss masih hidup, tetap spawn zombie biasa
            // Jika boss sudah mati, stop spawn (akan di-handle di OnBossZombieDied)
            if (bossSpawned && bossAlive && FindObjectOfType<GameManager>().GetDayCount() == 5)
            {
                // Tetap spawn zombie biasa
            }
            else if (bossSpawned && !bossAlive && FindObjectOfType<GameManager>().GetDayCount() == 5)
            {
                // Boss sudah mati, stop spawner di hari ke-5
                StopSpawning();
                yield break;
            }

            SpawnZombie();
            zombiesSpawnedThisNight++;
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

    void SpawnBossZombie()
    {
        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
        GameObject boss = Instantiate(bossZombiePrefab, spawnPos, Quaternion.identity);
        BossZombie bossScript = boss.GetComponent<BossZombie>();
        if (bossScript != null)
        {
            // Bisa set property khusus boss di sini jika perlu
        }
    }

    // Dipanggil oleh BossZombie saat mati
    public void OnBossZombieDied()
    {
        bossAlive = false;
        // Jika hari ke-5, stop spawner setelah boss mati
        int day = FindObjectOfType<GameManager>().GetDayCount();
        if (day == 5)
        {
            StopSpawning();
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
