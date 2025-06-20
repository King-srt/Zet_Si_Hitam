using UnityEngine;

public class BossZombie : ZombieAI
{
   public override void Die()
{
    if (isDead) return; // biar tidak double death
    base.Die();

    // Beritahu spawner
    ZombieSpawner[] spawners = FindObjectsOfType<ZombieSpawner>();
    foreach (var spawner in spawners)
    {
        spawner.OnBossZombieDied();
    }

   if (CompareTag("ZombieBoss") && GameManager.Instance != null)
{
    Debug.Log("🎯 Boss zombie mati, memanggil EndGame");
    GameManager.Instance.EndGame(true); // Victory!
}

}


}

