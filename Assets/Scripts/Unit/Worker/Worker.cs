using UnityEngine;

[RequireComponent(typeof(WorkerAnimatorController))]
public class Worker : BaseUnit
{
    private enum WorkerState { Idle, Mining, Returning }
    private WorkerState state = WorkerState.Idle;

    private Headquarter nearestHQ;
    private bool isReturningToHQ = false;
    private Vector2 previousMinePosition;

    [Header("Mining Settings")]
    public float miningRange = 1.5f;
    public float miningInterval = 1f;
    public int goldPerMine = 2;
    public int maxCarriedGold = 10;

    [Header("FX Settings")]
    public GameObject goldPickupPrefab;
    [Header("UI Menu")]
    public GameObject menuUI;

    [SerializeField] private int carriedGold = 0;

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

 private void OnEnable()
{
    GameManager.OnTimeChanged += OnTimeChangedHandler;
}

private void OnDisable()
{
    GameManager.OnTimeChanged -= OnTimeChangedHandler;
}



    protected override void Update()
    {
        base.Update();
        if (state == WorkerState.Returning && nearestHQ != null)
        {
            float distance = Vector2.Distance(transform.position, nearestHQ.transform.position);
            if (distance < 1f)
{
    nearestHQ.StoreGold(carriedGold);
    Debug.Log($"💰 Worker menyetorkan {carriedGold} ke HQ.");
    carriedGold = 0;

    if (GameManager.IsNight)
    {
        gameObject.SetActive(false); // Worker seolah tidur saat malam
    }
    else
    {
        SetTargetPosition(previousMinePosition);
        state = WorkerState.Mining;
    }
}
        }

        animatorController.SetWalking(isMoving);
        HandleMining();

        if (Input.GetKeyDown(KeyCode.E) && selectedUnit != null)
        {
            Debug.Log("Tombol E Ditekan Yeayyy");
            if (menuUI != null)
            {
                if (menuUI.activeSelf)
                {
                    CloseMenu();
                }
                else
                {
                    OpenMenu();
                }
            }
        }
    }

    private void HandleMining()
    {
        if (state != WorkerState.Mining)
        {
            animatorController.SetMining(false);
            return;
        }

        if (currentTarget == null || currentTarget.IsDepleted())
        {
            animatorController.SetMining(false);
            return;
        }

        float distance = Vector2.Distance(transform.position, currentTarget.transform.position);
        if (distance <= miningRange)
        {
            animatorController.SetMining(true); // Mulai animasi mining

            miningTimer += Time.deltaTime;
            if (miningTimer >= miningInterval)
            {
                if (carriedGold < maxCarriedGold)
                {
                    bool mined = currentTarget.MineGold(goldPerMine);
                    if (mined)
                    {
                        carriedGold += goldPerMine;
                        Debug.Log($"Worker mengangkut {goldPerMine}, total: {carriedGold}");

                        GameObject goldPickup = Instantiate(goldPickupPrefab, currentTarget.transform.position, Quaternion.identity);
                        goldPickup.GetComponent<GoldPickup>().target = transform;
                    }
                }

                if (carriedGold >= maxCarriedGold)
                {
                    nearestHQ = FindNearestHeadquarter();
                    if (nearestHQ != null)
                    {
                        previousMinePosition = GetPreciseMiningPosition(currentTarget);
                        SetTargetPosition(nearestHQ.transform.position);
                        animatorController.SetMining(false); // Stop animasi
                        state = WorkerState.Returning;
                    }
                }

                miningTimer = 0f;
            }
        }
        else
        {
            animatorController.SetMining(false); // Tidak dalam jangkauan
        }
    }



    public void SetMiningTarget(GoldMine mine)
    {
        currentTarget = mine;
        state = WorkerState.Mining;

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

    private void OnTimeChangedHandler(bool isNight)
{
    if (isNight)
    {
        // Saat malam, suruh worker kembali ke HQ
        nearestHQ = FindNearestHeadquarter();
        if (nearestHQ != null)
        {
            SetTargetPosition(nearestHQ.transform.position);
            state = WorkerState.Returning;
            Debug.Log("🌙 Malam tiba, Worker kembali ke HQ.");
        }
    }
    else
    {
        // Saat pagi, worker siap menambang lagi (jika sebelumnya disembunyikan)
        gameObject.SetActive(true);
        Debug.Log("☀️ Pagi tiba, Worker siap bekerja.");
    }
}

    public void OpenMenu()
    {
        menuUI.SetActive(true);
        menuUI.transform.position = transform.position + new Vector3(0, -1.5f, 0);
    }

    public void CloseMenu()
    {
        menuUI.SetActive(false);
    }

    private Headquarter FindNearestHeadquarter()
    {
        Headquarter[] hqs = GameObject.FindObjectsOfType<Headquarter>();
        Headquarter closest = null;
        float minDist = Mathf.Infinity;

        foreach (var hq in hqs)
        {
            float dist = Vector2.Distance(transform.position, hq.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = hq;
            }
        }

        return closest;
    }

    private Vector2 GetPreciseMiningPosition(GoldMine mine)
    {
        Collider2D mineCollider = mine.GetComponent<Collider2D>();
        if (mineCollider == null)
        {
            return mine.transform.position;
        }

        Vector2 closestPoint = mineCollider.ClosestPoint(transform.position);
        Vector2 direction = (closestPoint - (Vector2)transform.position).normalized;
        float safeDistance = 0.2f;
        return closestPoint - direction * safeDistance;
    }

}