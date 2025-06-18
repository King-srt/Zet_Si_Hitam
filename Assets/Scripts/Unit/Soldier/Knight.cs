using UnityEngine;

public class KnightUnit : SoldierUnit
{
    protected override void PerformAttack(BaseUnit target)
    {
        if (target != null)
        {
            target.TakeDamage(attackDamage);
            Debug.Log($"{gameObject.name} (Knight) menyerang {target.name} dengan pedang.");
        }
    }
}
