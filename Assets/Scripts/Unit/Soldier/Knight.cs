using UnityEngine;

public class Knight : SoldierUnit
{
    protected override void PerformAttack(BaseEnemy target)
    {
        if (target != null)
        {
            target.TakeDamage(attackDamage);
            Debug.Log($"{gameObject.name} (Knight) menyerang {target.name} dengan pedang.");
        }
    }
}
