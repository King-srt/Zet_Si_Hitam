using UnityEngine;
using System.Collections;

public class ZombieAI : MonoBehaviour
{
    public Transform targetBase;
    public float speed = 2f;
    public float attackRange = 1.5f;
    public float detectionRadius = 5f;
    public float retreatDistance = 10f;
    public ZombieSpawner spawner;

    private Animator animator;
    private Transform currentTarget;
    private bool isAttacking = false;
    private bool isRetreating = false;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(StartRetreatAfterDelay(30f));
        StartCoroutine(UpdateTargetEvery(1f)); // per detik
        currentTarget = targetBase;
    }

    void Update()
    {
        if (isDead) return;

        if (isRetreating)
        {
            Retreat();
            return;
        }

        if (AllTargetsDestroyed())
        {
            Die();
            return;
        }

        if (currentTarget == null)
            currentTarget = targetBase;

        if (currentTarget == null || currentTarget.GetComponent<BaseUnit>() == null)
        {
            animator.ResetTrigger("Attack");
            isAttacking = false;
        }

        DetectNearbyTarget();

        if (currentTarget == null) return;

        float distance = Vector2.Distance(transform.position, currentTarget.position);

        if (distance > attackRange)
            MoveToTarget();
        else
            AttackTarget();
    }

   void DetectNearbyTarget()
{
    BaseUnit[] targets = FindObjectsOfType<BaseUnit>();
    Transform closest = null;
    float minDistance = Mathf.Infinity;

    foreach (BaseUnit tu in targets)
    {
        if (tu == null) continue;

        float dist = Vector2.Distance(transform.position, tu.transform.position);
        if (dist < minDistance)
        {
            closest = tu.transform;
            minDistance = dist;
        }
    }

    // Kalau tidak ada target hidup, fallback ke base
    currentTarget = (closest != null) ? closest : targetBase;
}


    void MoveToTarget()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            animator.ResetTrigger("Attack");

        animator.SetBool("IsWalking", true);
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void AttackTarget()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetBool("IsWalking", false);
            animator.SetTrigger("Attack");

            BaseUnit tu = currentTarget.GetComponent<BaseUnit>();
            if (tu != null)
            {
                tu.TakeDamage(10);
            }

            StartCoroutine(ResetAttackCooldown());
        }
    }


    void Retreat()
    {
        Vector2 dir = (transform.position - targetBase.position).normalized;
        transform.position += (Vector3)(dir * speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetBase.position) > retreatDistance)
        {
            spawner?.OnZombieDestroyed(this);
            Destroy(gameObject);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        isAttacking = true;
        animator.SetBool("IsWalking", false);
        animator.SetTrigger("Die");

        currentTarget = null;

        if (AllTargetsDestroyed())
            FindObjectOfType<ZombieSpawner>()?.StopSpawning();

        spawner?.OnZombieDestroyed(this);

        StartCoroutine(DestroyAfterDeathAnim());
    }

    IEnumerator DestroyAfterDeathAnim()
    {
        yield return null;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Die"));
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);

        Destroy(gameObject);
    }


    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(1.0f);
        isAttacking = false;
    }

    IEnumerator StartRetreatAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isDead)
        {
            isRetreating = true;
            animator.SetBool("IsWalking", true);
        }
    }

    IEnumerator UpdateTargetEvery(float interval)
{
    while (!isDead)
    {
        DetectNearbyTarget();
        yield return new WaitForSeconds(interval);
    }
}
    bool AllTargetsDestroyed()
    {
        var targets = Object.FindObjectsByType<BaseUnit>(FindObjectsSortMode.None);
        return targets.Length == 0;
    }
}
