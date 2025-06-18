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
