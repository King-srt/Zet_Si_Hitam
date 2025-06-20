using UnityEngine;

public class BossZombie : ZombieAI
{
    public override void Die()
    {
        base.Die();

        // Beritahu spawner bahwa boss sudah mati
        ZombieSpawner[] spawners = FindObjectsOfType<ZombieSpawner>();
        foreach (var spawner in spawners)
        {
            spawner.OnBossZombieDied();
        }

        // Beritahu GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NotifyBossDied();
        }
    }
}

