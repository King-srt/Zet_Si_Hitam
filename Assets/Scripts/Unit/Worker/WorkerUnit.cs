using UnityEngine;

[RequireComponent(typeof(WorkerAnimatorController))]
public class WorkerUnit : BaseUnit
{
    [Header("Mining Settings")]
    public float miningRange = 1.5f;
    public float miningInterval = 2f;
    public int goldPerMine = 10;

    [Header("FX Settings")]
    public GameObject goldPickupPrefab;

    private float miningTimer = 0f;
    private GoldMine currentTarget;
    private WorkerAnimatorController animatorController;

    protected override void Start()
    {
        animatorController = GetComponent<WorkerAnimatorController>();

        Transform body = transform.Find("Body");
        if (body != null)
        {
            spriteRenderer = body.GetComponent<SpriteRenderer>();
            originalColor = spriteRenderer.color;
        }

        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        animatorController.SetWalking(isMoving);
        HandleMining();
    }

    private void HandleMining()
    {
        if (currentTarget == null || currentTarget.IsDepleted())
        {
            animatorController.SetMining(false);
            return;
        }

        float distance = Vector2.Distance(transform.position, currentTarget.transform.position);
        if (distance <= miningRange)
        {
            animatorController.SetMining(true);
            Debug.Log("Mining...");
            miningTimer += Time.deltaTime;
            if (miningTimer >= miningInterval)
            {
                bool mined = currentTarget.MineGold(goldPerMine);
                if (mined)
                {
                    GameObject goldPickup = Instantiate(goldPickupPrefab, currentTarget.transform.position, Quaternion.identity);
                    goldPickup.GetComponent<GoldPickup>().target = transform;
                }
                else
                {
                    animatorController.SetMining(false);
                }
                miningTimer = 0f;
            }
        }
        else
        {
            animatorController.SetMining(false);
        }
    }

    public void SetMiningTarget(GoldMine mine)
    {
        currentTarget = mine;

        Collider2D mineCollider = mine.GetComponent<Collider2D>();
        if (mineCollider == null)
        {
            Debug.LogWarning("GoldMine doesn't have a Collider2D!");
            SetTargetPosition(mine.transform.position);
            return;
        }


        Vector2 closestPoint = mineCollider.ClosestPoint(transform.position);
        Vector2 direction = (closestPoint - (Vector2)transform.position).normalized;

        float safeDistance = 0.2f;
        Vector2 stopPosition = closestPoint - direction * safeDistance;

        SetTargetPosition(stopPosition);
    }
}

