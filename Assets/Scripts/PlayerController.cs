using UnityEngine;
using UnityEngine.InputSystem; // Membutuhkan namespace ini

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f; // Kekuatan loncat
    private Rigidbody2D rb;

    // Variabel untuk mengecek apakah player sedang di dekat objek yang bisa diinteraksi
    private bool canInteract = false;
    private InteractableWorldItem nearbyItem;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Freeze rotation so the player doesn't tip over when moving
        rb.freezeRotation = true;
    }

    void Update()
    {
        float horizontalInput = 0f;
        bool jumpPressed = false;

        // Membaca input menggunakan New Input System (Keyboard)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                horizontalInput += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                horizontalInput -= 1f;
            
            // Cek tombol loncat (Space, W, atau Panah Atas)
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
                jumpPressed = true;
                
            // Cek interaksi dengan tombol E
            if (canInteract && Keyboard.current.eKey.wasPressedThisFrame && nearbyItem != null)
            {
                InventoryController invController = FindObjectOfType<InventoryController>();
                if (invController != null)
                {
                    invController.ReceiveWorldItem(nearbyItem);
                    nearbyItem.HideWorldItem(); // Sembunyikan SATU KESATUAN item dari dunia
                    nearbyItem = null;
                    canInteract = false;
                }
            }
        }

        // Terapkan kecepatan horizontal
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // Loncat: Hanya bisa loncat jika kecepatan Y hampir 0 (artinya sedang di tanah)
        if (jumpPressed && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // Mendeteksi saat player mendekati objek interaktif
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            canInteract = true;
            nearbyItem = collision.GetComponent<InteractableWorldItem>();
            if (nearbyItem != null)
            {
                nearbyItem.TogglePrompt(true);
            }
        }
    }

    // Mendeteksi saat player menjauhi objek interaktif
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            if (nearbyItem != null && nearbyItem.gameObject == collision.gameObject)
            {
                nearbyItem.TogglePrompt(false);
                nearbyItem = null;
                canInteract = false;
            }
        }
    }
}
