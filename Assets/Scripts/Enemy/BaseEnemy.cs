using UnityEngine;
using System.Collections;

public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float maxHP = 100f;
    public int damage = 10;
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float retreatDistance = 10f;
    public float maxForce = 5f;

    [Header("Targeting")]
    public string[] targetTags = { "Headquarter", "Worker", "Barrack", "Knight", "Archer", "Tower", "Wall" };
    public float updateTargetInterval = 1f;

    [Header("References")]
    public Animator animator;

    protected float currentHP;
    protected bool isDead = false;
    protected bool isAttacking = false;
    protected bool isRetreating = false;
    protected Vector2 velocity = Vector2.zero;
    protected Transform currentTarget;

    protected virtual void Awake()
    {
        currentHP = maxHP;
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        StartCoroutine(UpdateTargetRoutine(updateTargetInterval));
    }

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;
        currentHP -= amount;
        if (currentHP <= 0)
            Die();
    }

    public virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        isAttacking = false;
        velocity = Vector2.zero;
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetTrigger("Die");
        }
        StartCoroutine(DestroyAfterDeathAnimation());
    }

    protected virtual IEnumerator DestroyAfterDeathAnimation()
    {
        yield return null;
        if (animator != null)
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Die"));
            float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animLength);
        }
        Destroy(gameObject);
    }

    public virtual bool IsDead() => isDead;

    public virtual void StartRetreat()
    {
        if (!isDead && !isRetreating)
        {
            isRetreating = true;
            if (animator != null)
                animator.SetBool("IsWalking", true);
        }
    }

    protected virtual IEnumerator UpdateTargetRoutine(float interval)
    {
        while (!isDead)
        {
            UpdateTarget();
            yield return new WaitForSeconds(interval);
        }
    }

    // Cari target terdekat: BaseUnit atau BaseBuilding
    protected virtual void UpdateTarget()
    {
        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (string tag in targetTags)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in targets)
            {
                if (obj == null) continue;

                // Cek BaseUnit
                BaseUnit unit = obj.GetComponent<BaseUnit>();
                if (unit != null && unit.IsDead()) continue;

                // Cek BaseBuilding
                BaseBuilding building = obj.GetComponent<BaseBuilding>();
                if (building != null && building.IsDead()) continue;

                // Hanya pilih jika ada salah satu komponen
                if (unit == null && building == null) continue;

                float dist = Vector2.Distance(transform.position, obj.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = obj.transform;
                }
            }
        }

        currentTarget = closest;
    }

    // Steering behaviour untuk bergerak ke target
    public virtual void MoveToTarget(Transform target)
    {
        if (isAttacking) return;
        if (animator != null) animator.SetBool("IsWalking", true);

        Vector2 desired = ((Vector2)target.position - (Vector2)transform.position).normalized * moveSpeed;
        Vector2 steering = desired - velocity;
        steering = Vector2.ClampMagnitude(steering, maxForce);

        velocity += steering * Time.deltaTime;
        velocity = Vector2.ClampMagnitude(velocity, moveSpeed);

        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Flip arah hadap
        if (velocity.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(velocity.x);
            transform.localScale = scale;
        }
    }

    // Attack ke BaseUnit atau BaseBuilding
    public virtual void TryAttack()
    {
        if (isAttacking || isDead || currentTarget == null) return;

        // Cek BaseUnit
        BaseUnit targetUnit = currentTarget.GetComponent<BaseUnit>();
        if (targetUnit != null && !targetUnit.IsDead())
        {
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
        if (targetBuilding != null && !targetBuilding.IsDead())
        {
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

    protected virtual IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    // Steering behaviour untuk retreat
    public virtual void Retreat(Transform fromTarget, Vector2 retreatPoint)
    {
        Vector2 retreatDir;
        if (fromTarget != null)
            retreatDir = ((Vector2)transform.position - (Vector2)fromTarget.position).normalized;
        else
            retreatDir = ((Vector2)transform.position - retreatPoint).normalized;

        Vector2 desired = retreatDir * moveSpeed;
        Vector2 steering = desired - velocity;
        steering = Vector2.ClampMagnitude(steering, maxForce);

        velocity += steering * Time.deltaTime;
        velocity = Vector2.ClampMagnitude(velocity, moveSpeed);

        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Flip ke arah retreat
        if (velocity.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(velocity.x);
            transform.localScale = scale;
        }
    }

    // Ganti perhitungan distance di Update/AI dengan fungsi ini:
    protected float GetDistanceToTarget(Transform target)
    {
        if (target == null) return Mathf.Infinity;

        // Jika target BaseBuilding dan punya collider
        BaseBuilding building = target.GetComponent<BaseBuilding>();
        if (building != null)
        {
            Collider2D col = building.GetComponent<Collider2D>();
            if (col != null)
            {
                Vector2 closest = col.ClosestPoint(transform.position);
                // Tidak ada offset, zombie berhenti tepat di luar collider bangunan
                float dist = Vector2.Distance(transform.position, closest);
                return Mathf.Max(0f, dist + 1f); // atau -0.2f untuk lebih dekat lagi

            }
        }

        // Jika target BaseUnit dan punya collider (lebih dekat, misal offset kecil)
        BaseUnit unit = target.GetComponent<BaseUnit>();
        if (unit != null)
        {
            Collider2D col = unit.GetComponent<Collider2D>();
            if (col != null)
            {
                Vector2 closest = col.ClosestPoint(transform.position);
                // Offset agar zombie bisa lebih dekat ke unit (misal -0.1f)
                float dist = Vector2.Distance(transform.position, closest);
                return Mathf.Max(0f, dist + 1f);
            }
        }

        // Default: jarak ke pusat
        return Vector2.Distance(transform.position, target.position);
    }
}