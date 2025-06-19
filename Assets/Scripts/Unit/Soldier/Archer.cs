using UnityEngine;

public class ArcherUnit : SoldierUnit
{
    [Header("Projectile Settings")]
    public GameObject arrowPrefab;
    public Transform firePoint;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update(); // kita bisa pakai base logika, tapi override movement

        // Cegah mendekat ke musuh
        if (currentTarget != null && !currentTarget.IsDead())
        {
            float distance = Vector2.Distance(transform.position, currentTarget.transform.position);

            if (distance > attackRange)
            {
                // Jangan mendekat! Hanya idle atau cari musuh lain
                animator.SetBool("isWalking", false);
                SetTargetPosition(transform.position); // stay in place
            }
        }
    }
    protected override void PerformAttack(BaseEnemy target)
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack03");
        }

        if (arrowPrefab != null && firePoint != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
            Projectile proj = arrow.GetComponent<Projectile>();
            proj.Init(target.transform, attackDamage);

            Debug.Log($"{gameObject.name} (Archer) menembakkan panah ke {target.name}.");
        }
    }


}
