using UnityEngine;

public class ZombieAI : BaseEnemy
{
    public float retreatDelay = 30f;
    public ZombieSpawner spawner;

    private bool retreatTimerStarted = false;

    protected override void Start()
    {
        base.Start();

        if (!GameManager.IsNight)
        {
            StartRetreat();
        }
        else
        {
            StartRetreatTimer();
        }
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
    {
        if (animator != null)
            animator.SetBool("IsWalking", false);
        return;
    }

    // Cek target valid (unit atau building)
    bool targetValid = false;
    if (currentTarget != null)
    {
        var unit = currentTarget.GetComponent<BaseUnit>();
        var building = currentTarget.GetComponent<BaseBuilding>();
        if (unit != null && !unit.IsDead()) targetValid = true;
        if (building != null && !building.IsDead()) targetValid = true;
    }

    if (!targetValid)
    {
        UpdateTarget();
        if (currentTarget == null)
        {
            // Tidak ada target, zombie langsung mati
            Die();
            return;
        }
    }

    float distance = GetDistanceToTarget(currentTarget);

    if (distance > attackRange)
        MoveToTarget(currentTarget);
    else
        TryAttack();
}

    public override void TryAttack()
    {
        if (isAttacking || isDead || currentTarget == null) return;

        // Cek BaseUnit
        BaseUnit targetUnit = currentTarget.GetComponent<BaseUnit>();
        if (targetUnit != null)
        {
            if (targetUnit.IsDead())
            {
                UpdateTarget();
                return;
            }

            isAttacking = true;
            velocity = Vector2.zero;
            if (animator != null)
            {
                animator.SetBool("IsWalking", false);
                animator.SetTrigger("Attack");
            }

            targetUnit.TakeDamage(damage);

            StartCoroutine(ResetAttackCooldown());
            return;
        }

        // Cek BaseBuilding
        BaseBuilding targetBuilding = currentTarget.GetComponent<BaseBuilding>();
        if (targetBuilding != null)
        {
            if (targetBuilding.IsDead())
            {
                UpdateTarget();
                return;
            }

            isAttacking = true;
            velocity = Vector2.zero;
            if (animator != null)
            {
                animator.SetBool("IsWalking", false);
                animator.SetTrigger("Attack");
            }

            targetBuilding.TakeDamage(damage);

            StartCoroutine(ResetAttackCooldown());
            return;
        }

        // Jika target mati, cari target baru
        UpdateTarget();
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

        if (!isNight)
        {
            StartRetreat();
        }
        else
        {
            isRetreating = false;
            isAttacking = false;
            if (animator != null)
                animator.SetBool("IsWalking", false);

            StartRetreatTimer();
        }
    }

    private void StartRetreatTimer()
    {
        if (retreatTimerStarted)
            CancelInvoke(nameof(StartRetreat));

        retreatTimerStarted = true;
        Invoke(nameof(StartRetreat), retreatDelay);
    }

    public override void StartRetreat()
    {
        if (!isDead && !isRetreating)
        {
            isRetreating = true;
            if (animator != null)
                animator.SetBool("IsWalking", true);

            CancelInvoke(nameof(StartRetreat));
            retreatTimerStarted = false;
        }
    }
}