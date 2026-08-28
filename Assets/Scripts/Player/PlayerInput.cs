using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public float HorizontalInput { get; private set; }
    public bool JumpPressedThisFrame { get; private set; }
    public bool JumpIsHeld { get; private set; }
    public bool InteractPressedThisFrame { get; private set; }

    void Update()
    {
        HorizontalInput = 0f;
        JumpPressedThisFrame = false;
        JumpIsHeld = false;
        InteractPressedThisFrame = false;

        if (GameplayManager.Instance != null && GameplayManager.Instance.IsGameOver) return;
        if (Time.timeScale == 0f) return;

        if (Keyboard.current == null) return;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            HorizontalInput += 1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            HorizontalInput -= 1f;

        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
            JumpPressedThisFrame = true;

        if (Keyboard.current.spaceKey.isPressed || Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            JumpIsHeld = true;

        if (Keyboard.current.eKey.wasPressedThisFrame)
            InteractPressedThisFrame = true;
    }
}
