using UnityEngine;

public class KnightUnit : SoldierUnit
{


void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
protected override void PerformAttack(BaseUnit target)
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack01");
        }

        target.TakeDamage(attackDamage);
        Debug.Log($"{gameObject.name} (Knight) menyerang {target.name} dengan pedang.");

    }
}