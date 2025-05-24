using UnityEngine;
using System.Collections;

public class TargetUnit : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHP = 100;
    private int currentHP;

    [Header("Optional")]
    public bool isCritical = false; // contoh: Base bisa diset lebih penting

    [Header("Movement Settings")]
    public float moveRadius = 4f;         // Radius gerak acak
    public float moveInterval = 1f;       // Waktu antar gerakan
    public float moveSpeed = 3f;          // Kecepatan gerak

    private Vector3 targetPosition;

    private void Start()
    {
        currentHP = maxHP;
        targetPosition = transform.position;
        StartCoroutine(RandomMovement());
    }

    private void Update()
    {
        // Gerakkan menuju targetPosition secara smooth
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed!");
        Destroy(gameObject);
    }

    public bool IsDestroyed()
    {
        return currentHP <= 0;
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public float GetHealthPercent()
    {
        return (float)currentHP / maxHP;
    }

    // Coroutine untuk gerakan acak
    IEnumerator RandomMovement()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveInterval);

            // Gerak ke posisi acak dalam radius
            Vector2 randomOffset = Random.insideUnitCircle * moveRadius;
            targetPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
        }
    }
}
