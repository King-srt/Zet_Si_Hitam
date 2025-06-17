using UnityEngine;
using System.Collections;

public abstract class SoldierUnit : BaseUnit
{
    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;

    [Header("Target Settings")]
    public string[] enemyTags = { "Zombie" };  // kamu bisa sesuaikan ini

    protected float lastAttackTime = 0f;
    protected BaseUnit currentTarget;
    protected Animator animator;
    protected override void Start()
    {
        base.Start();
        StartCoroutine(UpdateTargetRoutine(1f));
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
                    PerformAttack(currentTarget);
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                SetTargetPosition(currentTarget.transform.position);
            }
        }
    }

    public void SetTarget(BaseUnit target)
    {
        currentTarget = target;
    }

    protected abstract void PerformAttack(BaseUnit target);

    IEnumerator UpdateTargetRoutine(float interval)
    {
        while (true)
        {
            if (currentTarget == null || currentTarget.IsDead())
            {
                currentTarget = FindClosestEnemy();
            }
            yield return new WaitForSeconds(interval);
        }
    }

    BaseUnit FindClosestEnemy()
    {
        float closestDistance = Mathf.Infinity;
        BaseUnit closest = null;

        foreach (string tag in enemyTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in enemies)
            {
                if (obj == null) continue;
                BaseUnit unit = obj.GetComponent<BaseUnit>();
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
