using UnityEngine;

public class Archer : SoldierUnit
{

    private Tower currentTower;

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
            animator.SetTrigger("attack");
        }

        if (arrowPrefab != null && firePoint != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
            Projectile proj = arrow.GetComponent<Projectile>();
            proj.Init(target.transform, attackDamage);

            Debug.Log($"{gameObject.name} (Archer) menembakkan panah ke {target.name}.");
        }
    }

    public void EnterTower(Tower tower)
    {
        currentTower = tower;
        transform.position = tower.GetArcherPoint().position;
        // Tambahan: nonaktifkan movement/shooting jika perlu
    }

    public void ExitTower()
    {
        if (currentTower == null) return;

        // Misal: keluar ke posisi di bawah tower
        Vector3 exitPos = currentTower.transform.position + Vector3.down;
        transform.position = exitPos;

        currentTower = null;
    }

    public bool IsInTower()
    {
        return currentTower != null;
    }
}
