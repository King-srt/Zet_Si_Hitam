using UnityEngine;

public class ZombieAI : BaseEnemy
{
    public float retreatDelay = 30f;
    public ZombieSpawner spawner;

    protected override void Start()
    {
        base.Start();
        Invoke(nameof(StartRetreat), retreatDelay);
    }

    void Update()
    {
        if (isDead) return;

        if (isRetreating)
        {
            Retreat(currentTarget, GetRetreatPoint());
            return;
        }

        if (!GameManager.IsNight)
            return;

        if (currentTarget == null || currentTarget.GetComponent<BaseUnit>() == null || currentTarget.GetComponent<BaseUnit>().IsDead())
        {
            UpdateTarget();
            if (currentTarget == null) return;
        }

        float distance = Vector2.Distance(transform.position, currentTarget.position);

        if (distance > attackRange)
            MoveToTarget(currentTarget);
        else
            TryAttack();
    }

    Vector2 GetRetreatPoint()
    {
        return spawner != null ? spawner.transform.position : transform.position + Vector3.left * 5f;
    }

    private void OnEnable()
    {
        GameManager.OnTimeChanged += HandleTimeChange;
    }

    private void OnDisable()
    {
        GameManager.OnTimeChanged -= HandleTimeChange;
    }

    private void HandleTimeChange(bool isNight)
    {
        if (isDead) return;

        if (!isNight) // Daytime: retreat
        {
            StartRetreat();
        }
        else // Nighttime: attack
        {
            isRetreating = false;
            isAttacking = false;
            if (animator != null)
                animator.SetBool("IsWalking", false);
        }
    }
}