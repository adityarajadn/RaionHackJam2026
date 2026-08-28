using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAudio : MonoBehaviour
{
    private float stepTimer = 0f;
    private float stepInterval = 0.35f;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (playerMovement.HasJumpedThisFrame)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("jump");
        }

        if (playerMovement.HasWalkedThisFrame)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("step");
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }
}
