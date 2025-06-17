using UnityEngine;
using System.Collections;

public class ZombieAI : MonoBehaviour
{
    [Header("Targeting & Movement")]
    public float attackRange = 1.5f;
    public float retreatDistance = 10f;

    [Header("Steering Behavior")]
    public float maxSpeed = 2f;
    public float maxForce = 5f;
    private Vector2 velocity = Vector2.zero;

    [Header("Zombie Settings")]
    public float attackCooldown = 1.0f;
    public float retreatDelay = 30f;
    public int damage = 10;

    [Header("References")]
    public ZombieSpawner spawner;
    public string[] targetTags = { "Knight", "Archer" };

    private Animator animator;
    private Transform currentTarget;
    private bool isAttacking = false;
    private bool isRetreating = false;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        StartCoroutine(UpdateTargetRoutine(1f));
        Invoke(nameof(StartRetreat), retreatDelay);
    }

   void Update()
{
    if (isDead)
        return;

    if (isRetreating)
    {
        Retreat();
        return;
    }

    // FIX: Zombies attack at night, retreat at day
    if (!GameManager.IsNight)
        return;

    if (currentTarget == null || currentTarget.GetComponent<BaseUnit>() == null || currentTarget.GetComponent<BaseUnit>().IsDead())
    {
        UpdateTarget();
        if (currentTarget == null) return;
    }

    float distance = Vector2.Distance(transform.position, currentTarget.position);

    if (distance > attackRange)
        MoveToTarget();
    else
        TryAttack();
}



    void MoveToTarget()
    {
        if (isAttacking) return;

        animator.SetBool("IsWalking", true);

        Vector2 desired = ((Vector2)currentTarget.position - (Vector2)transform.position).normalized * maxSpeed;
        Vector2 steering = desired - velocity;
        steering = Vector2.ClampMagnitude(steering, maxForce);

        velocity += steering * Time.deltaTime;
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Flip arah hadap
        if (velocity.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(velocity.x);
            transform.localScale = scale;
        }
    }

    void TryAttack()
    {
        if (isAttacking || isDead || currentTarget == null) return;

        BaseUnit targetUnit = currentTarget.GetComponent<BaseUnit>();
        if (targetUnit == null || targetUnit.IsDead()) // Pastikan unit masih hidup
        {
            UpdateTarget();
            return;
        }

        isAttacking = true;
        velocity = Vector2.zero;
        animator.SetBool("IsWalking", false);
        animator.SetTrigger("Attack");

        targetUnit.TakeDamage(damage);

        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void Retreat()
{
    Vector2 retreatDir;
    if (currentTarget != null)
    {
        // Move away from the target
        retreatDir = ((Vector2)transform.position - (Vector2)currentTarget.position).normalized;
    }
    else
    {
        // Fallback: move away from spawner or just left
        retreatDir = ((Vector2)transform.position - GetRetreatPoint()).normalized;
    }

    // Steering behaviour (smooth acceleration)
    Vector2 desired = retreatDir * maxSpeed;
    Vector2 steering = desired - velocity;
    steering = Vector2.ClampMagnitude(steering, maxForce);

    velocity += steering * Time.deltaTime;
    velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

    transform.position += (Vector3)(velocity * Time.deltaTime);

    // Flip to face retreat direction
    if (velocity.x != 0)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(velocity.x);
        transform.localScale = scale;
    }

    // Optional: destroy zombie if far enough from target/spawner
    float retreatFrom = currentTarget != null ? Vector2.Distance(transform.position, currentTarget.position)
                                              : Vector2.Distance(transform.position, GetRetreatPoint());
    if (retreatFrom > retreatDistance)
    {
        spawner?.OnZombieDestroyed(this);
        Destroy(gameObject);
    }
}


    Vector2 GetRetreatPoint()
    {
        return spawner != null ? spawner.transform.position : transform.position + Vector3.left * 5f;
    }

    public void StartRetreat()
    {
        if (!isDead && !isRetreating)
        {
            isRetreating = true;
            animator.SetBool("IsWalking", true);
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        isAttacking = false;
        velocity = Vector2.zero;

        animator.SetBool("IsWalking", false);
        animator.SetTrigger("Die");

        spawner?.OnZombieDestroyed(this);

        StartCoroutine(DestroyAfterDeathAnimation());
    }

    IEnumerator DestroyAfterDeathAnimation()
    {
        yield return null;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Die"));
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);

        Destroy(gameObject);
    }

    IEnumerator UpdateTargetRoutine(float interval)
    {
        while (!isDead)
        {
            UpdateTarget();
            yield return new WaitForSeconds(interval);
        }
    }

    void UpdateTarget()
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

        if (closest == null && !isRetreating && GameManager.IsNight)
        {
        // Tidak ada target ditemukan, zombie idle
        animator.SetBool("IsWalking", false);
        }

    }

// ini ngatur zombie untuk mundur otomatis saat siang , dan maju pas malam
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
        animator.SetBool("IsWalking", false); // Stop walking animation if needed
    }
}


}
