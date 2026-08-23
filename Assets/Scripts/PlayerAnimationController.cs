using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;

    // Menggunakan StringToHash untuk performa yang lebih baik
    private readonly int isRunningHash = Animator.StringToHash("isRunning");
    private readonly int isInteractingHash = Animator.StringToHash("isInteracting");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Panggil fungsi ini dari script movement player kamu (misal pas tombol jalan ditekan)
    /// </summary>
    public void SetRunning(bool isRunning)
    {
        if (animator != null)
        {
            animator.SetBool(isRunningHash, isRunning);
        }
    }

    /// <summary>
    /// Panggil fungsi ini pas player lagi interaksi sama item atau NPC
    /// </summary>
    public void SetInteracting(bool isInteracting)
    {
        if (animator != null)
        {
            animator.SetBool(isInteractingHash, isInteracting);
            
            // Kalau diset true, kita otomatis kembalikan ke false setelah animasi selesai
            if (isInteracting && gameObject.activeInHierarchy)
            {
                StartCoroutine(ResetInteractingRoutine());
            }
        }
    }

    private IEnumerator ResetInteractingRoutine()
    {
        // Tunggu 0.5 detik (sesuaikan dengan panjang durasi animasi interaksi kamu)
        yield return new WaitForSeconds(0.5f);
        
        if (animator != null)
        {
            animator.SetBool(isInteractingHash, false);
        }
    }
}
