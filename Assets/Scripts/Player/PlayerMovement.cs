using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f; 
    [SerializeField] private float fallMultiplier = 2.5f; 
    [SerializeField] private float lowJumpMultiplier = 2f; 
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    [SerializeField] private GameObject shadowObject;

    private Vector2 velocity = Vector2.zero;
    private bool canDoubleJump = false;
    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private PlayerAnimationController playerAnimationController;

    public bool IsGrounded { get; private set; }
    public bool HasWalkedThisFrame { get; private set; }
    public bool HasJumpedThisFrame { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        playerAnimationController = GetComponent<PlayerAnimationController>();
        rb.freezeRotation = true;

        if (shadowObject == null)
        {
            Transform shadow = transform.Find("shadow");
            if (shadow == null) shadow = transform.Find("Shadow");
            if (shadow != null) shadowObject = shadow.gameObject;
        }
    }

    void Update()
    {
        if (GameplayManager.Instance != null && GameplayManager.Instance.IsGameOver) return;
        if (Time.timeScale == 0f) return;

        HasWalkedThisFrame = false;
        HasJumpedThisFrame = false;

        float horizontalInput = playerInput.HorizontalInput;

        if (horizontalInput > 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else if (horizontalInput < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        if (playerAnimationController != null)
        {
            playerAnimationController.SetRunning(horizontalInput != 0f);
        }

        float currentMoveSpeed = moveSpeed;
        float currentJumpForce = jumpForce;
        if (GameplayManager.Instance != null)
        {
            float weightRatio = GameplayManager.Instance.TotalWeight / GameplayManager.Instance.MaxWeight;
            weightRatio = Mathf.Clamp01(weightRatio); 
            float minSpeed = moveSpeed * 0.2f; 
            currentMoveSpeed = Mathf.Lerp(moveSpeed, minSpeed, weightRatio);
            float minJumpForce = jumpForce * 0.5f; 
            currentJumpForce = Mathf.Lerp(jumpForce, minJumpForce, weightRatio);
        }

        Vector2 targetVelocity = new Vector2(horizontalInput * currentMoveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocity, movementSmoothing);

        IsGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.01f;

        if (shadowObject != null)
        {
            if (shadowObject.activeSelf != IsGrounded)
            {
                shadowObject.SetActive(IsGrounded);
            }
        }

        if (IsGrounded)
        {
            canDoubleJump = true;
            if (Mathf.Abs(horizontalInput) > 0.1f)
            {
                HasWalkedThisFrame = true;
            }
        }

        if (playerInput.JumpPressedThisFrame)
        {
            if (IsGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpForce);
                HasJumpedThisFrame = true;
            }
            else if (canDoubleJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpForce);
                canDoubleJump = false;
                HasJumpedThisFrame = true;
            }
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !playerInput.JumpIsHeld)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (playerAnimationController != null)
        {
            playerAnimationController.SetGrounded(IsGrounded);
            playerAnimationController.SetYVelocity(rb.linearVelocity.y);
        }
    }
}
