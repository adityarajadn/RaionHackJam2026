using UnityEngine;
using UnityEngine.InputSystem; // Membutuhkan namespace ini

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f; // Kekuatan loncat
    private bool canDoubleJump = false;
    private Rigidbody2D rb;

    // Variabel untuk mengecek apakah player sedang di dekat objek yang bisa diinteraksi
    private System.Collections.Generic.List<Interactable> nearbyInteractables = new System.Collections.Generic.List<Interactable>();

    private PlayerAnimationController playerAnimationController;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimationController = GetComponent<PlayerAnimationController>();
        // Freeze rotation so the player doesn't tip over when moving
        rb.freezeRotation = true;
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

        // Terapkan kecepatan horizontal
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // Loncat dan Double Jump
        bool isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.01f;

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        if (jumpPressed)
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (canDoubleJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                canDoubleJump = false;
            }
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
