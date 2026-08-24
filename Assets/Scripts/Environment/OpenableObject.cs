using UnityEngine;

public class OpenableObject : Interactable
{
    [Header("Sprites")]
    [Tooltip("GameObject atau SpriteRenderer untuk state tertutup")]
    public GameObject closedSprite;
    [Tooltip("GameObject atau SpriteRenderer untuk state terbuka")]
    public GameObject openSprite;

    private bool isOpen = false;

    protected override void Awake()
    {
        base.Awake();
        // Pastikan sprite yang aktif sesuai state awal
        UpdateSpriteState();
    }

    public override void Interact()
    {
        isOpen = !isOpen;
        UpdateSpriteState();
    }

    private void UpdateSpriteState()
    {
        if (closedSprite != null)
        {
            closedSprite.SetActive(!isOpen);
        }
        
        if (openSprite != null)
        {
            openSprite.SetActive(isOpen);
        }
    }
}
