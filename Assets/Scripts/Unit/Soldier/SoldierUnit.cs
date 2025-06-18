using UnityEngine;

public abstract class SoldierUnit : BaseUnit
{
    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;

    protected float lastAttackTime = 0f;
    protected BaseUnit currentTarget;

    protected Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        if (currentTarget != null && !currentTarget.IsDead())
        {
            float distance = Vector2.Distance(transform.position, currentTarget.transform.position);

            if (distance <= attackRange)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    animator.SetTrigger("attack"); // ⬅️ Trigger animasi serang
                    PerformAttack(currentTarget);
                    lastAttackTime = Time.time;
                }

                animator.SetBool("isWalking", false); // ⬅️ Berhenti jalan saat menyerang
            }
            else
            {
                SetTargetPosition(currentTarget.transform.position);
                animator.SetBool("isWalking", true); // ⬅️ Jalan ke target
            }
        }
        else
        {
            animator.SetBool("isWalking", false); // Tidak jalan jika tidak ada target
        }
    }

    public void SetTarget(BaseUnit target)
    {
        currentTarget = target;
    }

    protected abstract void PerformAttack(BaseUnit target);

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (!IsDead())
        {
            animator.SetTrigger("hurt"); // ⬅️ Trigger animasi kena hit
        }
    }

    public override void Die()
    {
        base.Die();
        animator.SetTrigger("death"); // ⬅️ Trigger animasi mati
    }
}
