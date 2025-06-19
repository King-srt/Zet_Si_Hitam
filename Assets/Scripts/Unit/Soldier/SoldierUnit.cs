using UnityEngine;
using System.Collections;

public abstract class SoldierUnit : BaseUnit
{
    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;

    [Header("Target Settings")]
    public string[] enemyTags = { "Zombie", "ZombieKroco" };

    protected float lastAttackTime = 0f;
    protected BaseEnemy currentTarget;
    protected Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        StartCoroutine(UpdateTargetRoutine(0.1f));
    }

    protected virtual void OnEnable()
    {
        GameManager.OnTimeChanged += HandleTimeChanged;
    }

    protected virtual void OnDisable()
    {
        GameManager.OnTimeChanged -= HandleTimeChanged;
    }

    private void HandleTimeChanged(bool isNight)
    {
        if (!isNight)
        {
            animator.SetBool("isWalking", false);
            currentTarget = null;
        }
        else
        {
            currentTarget = FindClosestEnemy();
        }
    }

    protected override void Update()
    {
        base.Update();

        // Only attack at night
        if (!GameManager.IsNight)
        {
            animator.SetBool("isWalking", false);
            return;
        }

        if (currentTarget != null && !currentTarget.IsDead())
        {
            float distance = Vector2.Distance(transform.position, currentTarget.transform.position);

            if (distance <= attackRange)
            {
                // Face the target
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * Mathf.Sign(currentTarget.transform.position.x - transform.position.x);
                transform.localScale = scale;

                animator.SetBool("isWalking", false);

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    animator.SetTrigger("attack");
                    PerformAttack(currentTarget);
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * Mathf.Sign(currentTarget.transform.position.x - transform.position.x);
                transform.localScale = scale;

                SetTargetPosition(currentTarget.transform.position);
                animator.SetBool("isWalking", true);
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    public void SetTarget(BaseEnemy target)
    {
        currentTarget = target;
    }

    protected abstract void PerformAttack(BaseEnemy target);

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (!IsDead())
        {
            animator.SetTrigger("hurt");
        }
    }

    public override void Die()
{
    base.Die(); // Panggil base untuk log atau penanda kematian
    animator.SetTrigger("death");
    StartCoroutine(DestroyAfterDeathAnimation());
}

private IEnumerator DestroyAfterDeathAnimation()
{
    Debug.Log($"{gameObject.name} menunggu animasi 'death'");

    // Tunggu sampai animasi state bernama "death" dimulai
    yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("death"));

    float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
    Debug.Log($"{gameObject.name} animasi death durasi: {animLength}");

    if (animLength <= 0f)
    {
        animLength = 1f; // fallback
        Debug.LogWarning($"{gameObject.name} animasi death durasinya 0, fallback ke 1 detik");
    }

    yield return new WaitForSeconds(animLength);
    Debug.Log($"{gameObject.name} menghancurkan diri setelah animasi death");
    Destroy(gameObject);
}

IEnumerator UpdateTargetRoutine(float interval)
{
    while (true)
    {
        if (!GameManager.IsNight)
        {
            currentTarget = null;
        }
        else
        {
            BaseEnemy closest = FindClosestEnemy();
            if (closest != null)
            {
                float distToClosest = Vector2.Distance(transform.position, closest.transform.position);
                float distToCurrent = currentTarget != null ? Vector2.Distance(transform.position, currentTarget.transform.position) : Mathf.Infinity;

                // Ganti target jika:
                // - currentTarget null
                // - currentTarget sudah mati
                // - closest lebih dekat daripada current
                if (currentTarget == null || currentTarget.IsDead() || distToClosest < distToCurrent)
                {
                    currentTarget = closest;
                }
            }
            else
            {
                currentTarget = null;
            }
        }

        yield return new WaitForSeconds(interval);
    }
}


    BaseEnemy FindClosestEnemy()
    {
        float closestDistance = Mathf.Infinity;
        BaseEnemy closest = null;

        foreach (string tag in enemyTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in enemies)
            {
                if (obj == null) continue;
                BaseEnemy unit = obj.GetComponent<BaseEnemy>();
                if (unit == null || unit.IsDead()) continue;

                float dist = Vector2.Distance(transform.position, obj.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = unit;
                }
            }
        }

        return closest;
    }
}
