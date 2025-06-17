using UnityEngine;

public class Headquarter : MonoBehaviour
{
    [Header("Unit Settings")]
    public GameObject workerPrefab;
    public Transform spawnPoint;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    public static event System.Action OnHQDestroyed;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TrainWorker()
    {
        if (workerPrefab != null && spawnPoint != null)
        {
            Instantiate(workerPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("HQ Destroyed!");
        OnHQDestroyed?.Invoke();
        Destroy(gameObject);
    }
}
