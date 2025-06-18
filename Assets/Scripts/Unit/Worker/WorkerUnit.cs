using UnityEngine;

[RequireComponent(typeof(WorkerAnimatorController))]
public class WorkerUnit : BaseUnit
{
    [Header("Mining Settings")]
    public float miningRange = 1.5f;
    public float miningInterval = 2f;
    public int goldPerMine = 10;

    [Header("UI Menu")]
    public GameObject menuUI;

    private float miningTimer = 0f;
    private GoldMine currentTarget;
    private WorkerAnimatorController animatorController;

    protected override void Start()
    {
        // Ambil animator controller
        animatorController = GetComponent<WorkerAnimatorController>();

        // Ambil spriteRenderer dari child "Body"
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

        // Sync animasi jalan
        animatorController.SetWalking(isMoving);

        // Cek dan proses mining
        HandleMining();
    }

    private void HandleMining()
    {
        if (currentTarget == null || currentTarget.IsDepleted())
        {
            animatorController.SetMining(false);
            return; // Prevents null reference below
        }

        float distance = Vector2.Distance(transform.position, currentTarget.transform.position);
        if (distance <= miningRange)
        {
            miningTimer += Time.deltaTime;
            if (miningTimer >= miningInterval)
            {
                bool mined = currentTarget.MineGold(goldPerMine);
                if (mined)
                {
                    animatorController.SetMining(true);
                    Debug.Log("Mining...");
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
        SetTargetPosition(mine.transform.position);
    }

    // void OnMouseDown()
    // {
    //     // Toggle menu UI
    //     if (menuUI != null)
    //     {
    //         if (menuUI.activeSelf)
    //         {
    //             CloseMenu();
    //         }
    //         else
    //         {
    //             OpenMenu();
    //         }
    //     }
    // }

    // public void OpenMenu()
    // {
    //     menuUI.SetActive(true);
    //     menuUI.transform.position = transform.position + new Vector3(0, -1.5f, 0);
    // }

    // public void CloseMenu()
    // {
    //     menuUI.SetActive(false);
    // }
}

