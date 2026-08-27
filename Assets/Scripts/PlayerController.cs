using UnityEngine;
using UnityEngine.InputSystem; // Membutuhkan namespace ini

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f; // Disesuaikan untuk gravitasi yang lebih berat
    [SerializeField] private float fallMultiplier = 2.5f; // Semakin besar semakin cepat jatuh
    [SerializeField] private float lowJumpMultiplier = 2f; // Semakin besar semakin cepat turun jika tombol dilepas
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    [SerializeField] private GameObject shadowObject;
    private Vector2 velocity = Vector2.zero;
    private bool canDoubleJump = false;
    private Rigidbody2D rb;

    // Variabel untuk mengecek apakah player sedang di dekat objek yang bisa diinteraksi
    private System.Collections.Generic.List<Interactable> nearbyInteractables = new System.Collections.Generic.List<Interactable>();

    private PlayerAnimationController playerAnimationController;
    private float stepTimer = 0f;
    private float stepInterval = 0.35f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimationController = GetComponent<PlayerAnimationController>();
        // Freeze rotation so the player doesn't tip over when moving
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
        if (GameplayManager.Instance != null && GameplayManager.Instance.isGameOver) return;
        if (Time.timeScale == 0f) return;

        float horizontalInput = 0f;
        bool jumpPressed = false;

        // Membaca input menggunakan New Input System (Keyboard)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                horizontalInput += 1f;
            }
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                horizontalInput -= 1f;
            }
            
            if (playerAnimationController != null)
            {
                playerAnimationController.SetRunning(horizontalInput != 0f);
            }

            // Flip object (dan child-childnya) sesuai arah gerak
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
            
            // Cek tombol loncat (Space, W, atau Panah Atas)
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
                jumpPressed = true;
                
            // Cek interaksi dengan tombol E
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                // Bersihkan null references jika ada objek yang hancur
                nearbyInteractables.RemoveAll(i => i == null);

                if (nearbyInteractables.Count > 0)
                {
                    if (playerAnimationController != null) playerAnimationController.SetInteracting(true);
                    
                    // Kita kumpulkan semua item untuk diambil sekaligus
                    bool hasItem = false;
                    System.Collections.Generic.List<Interactable> toInteract = new System.Collections.Generic.List<Interactable>();
                    
                    foreach (var interactable in nearbyInteractables)
                    {
                        if (interactable is InteractableWorldItem)
                        {
                            toInteract.Add(interactable);
                            hasItem = true;
                        }
                    }

                    // Jika ada item, ambil semua item tersebut ke staging area
                    if (hasItem)
                    {
                        foreach (var item in toInteract)
                        {
                            item.Interact();
                        }
                        // Hapus dari daftar di sekitar karena sudah diambil
                        nearbyInteractables.RemoveAll(i => i is InteractableWorldItem);
                    }
                    else
                    {
                        // Jika tidak ada item, interaksi dengan 1 objek lain yang terdekat (misal: pintu, next area)
                        nearbyInteractables[0].Interact();
                    }
                }
            }
        }

        // Hitung kecepatan berdasarkan berat inventaris
        float currentMoveSpeed = moveSpeed;
        if (GameplayManager.Instance != null)
        {
            float weightRatio = GameplayManager.Instance.totalWeight / GameplayManager.Instance.maxWeight;
            weightRatio = Mathf.Clamp01(weightRatio); // Pastikan nilainya antara 0 dan 1
            float minSpeed = moveSpeed * 0.2f; // Kecepatan minimal 20% dari kecepatan asli saat beban penuh
            currentMoveSpeed = Mathf.Lerp(moveSpeed, minSpeed, weightRatio);
        }

        // Terapkan kecepatan horizontal dengan efek smoothing (halus)
        Vector2 targetVelocity = new Vector2(horizontalInput * currentMoveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocity, movementSmoothing);

        // Loncat dan Double Jump
        bool isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.01f;

        if (shadowObject != null)
        {
            if (shadowObject.activeSelf != isGrounded)
            {
                shadowObject.SetActive(isGrounded);
            }
        }

        if (isGrounded)
        {
            canDoubleJump = true;

            // Logika footstep
            if (Mathf.Abs(horizontalInput) > 0.1f)
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

        if (jumpPressed)
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("jump");
            }
            else if (canDoubleJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                canDoubleJump = false;
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("jump");
            }
        }

        // Bikin jatuhnya lebih tajam (nggak kaya di bulan)
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !(Keyboard.current.spaceKey.isPressed || Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed))
        {
            // Tombol dilepas sebelum mencapai titik tertinggi -> potong loncatan
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (playerAnimationController != null)
        {
            playerAnimationController.SetGrounded(isGrounded);
            playerAnimationController.SetYVelocity(rb.linearVelocity.y);
        }
    }

    // Mendeteksi saat player mendekati objek interaktif
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();
        if (interactable != null)
        {
            if (!nearbyInteractables.Contains(interactable))
            {
                nearbyInteractables.Add(interactable);
                interactable.TogglePrompt(true);
            }
        }
    }

    // Mendeteksi saat player menjauhi objek interaktif
    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();
        if (interactable != null && nearbyInteractables.Contains(interactable))
        {
            interactable.TogglePrompt(false);
            nearbyInteractables.Remove(interactable);
        }
    }
}
