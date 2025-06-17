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

                BaseUnit unit = obj.GetComponent<BaseUnit>();
                if (unit != null && unit.IsDead()) continue;

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

    // Movement ke target
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

    // Attack ke target
    public virtual void TryAttack()
    {
        if (isAttacking || isDead || currentTarget == null) return;

        BaseUnit targetUnit = currentTarget.GetComponent<BaseUnit>();
        if (targetUnit == null || targetUnit.IsDead())
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
    }

    protected virtual IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    // Retreat dari target
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
}
