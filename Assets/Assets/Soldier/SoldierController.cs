using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Vector3 targetPos;
    private Animator animator;
    private bool isMoving = false;

    void Start()
    {
        targetPos = transform.position;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Klik kiri = jalan ke arah klik
        if (Input.GetMouseButtonDown(0))
        {
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos.z = 0;
            isMoving = true;
        }

        // Gerak ke target
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            animator.Play("Walk");

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                isMoving = false;
                animator.Play("Idle");
            }
        }
    }
}

