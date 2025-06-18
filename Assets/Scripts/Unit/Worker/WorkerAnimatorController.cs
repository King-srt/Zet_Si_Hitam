using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WorkerAnimatorController : MonoBehaviour
{
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetWalking(bool walking)
    {
        animator.SetBool("isWalking", walking);
    }

    public void SetMining(bool mining)
    {
        animator.SetBool("isMining", mining);
    }
}
