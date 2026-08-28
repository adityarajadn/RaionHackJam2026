using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private readonly int isRunningHash = Animator.StringToHash("isRunning");
    private readonly int isInteractingHash = Animator.StringToHash("isInteracting");
    private readonly int isGroundedHash = Animator.StringToHash("isGrounded");
    private readonly int yVelocityHash = Animator.StringToHash("yVelocity");
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void SetRunning(bool isRunning)
    {
        if (animator != null)
        {
            animator.SetBool(isRunningHash, isRunning);
        }
    }
    public void SetGrounded(bool isGrounded)
    {
        if (animator != null)
        {
            animator.SetBool(isGroundedHash, isGrounded);
        }
    }
    public void SetYVelocity(float yVelocity)
    {
        if (animator != null)
        {
            animator.SetFloat(yVelocityHash, yVelocity);
        }
    }
    public void SetInteracting(bool isInteracting)
    {
        if (animator != null)
        {
            animator.SetBool(isInteractingHash, isInteracting);
            if (isInteracting && gameObject.activeInHierarchy)
            {
                StartCoroutine(ResetInteractingRoutine());
            }
        }
    }
    private IEnumerator ResetInteractingRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        if (animator != null)
        {
            animator.SetBool(isInteractingHash, false);
        }
    }
}
