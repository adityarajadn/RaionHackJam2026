using UnityEngine;

public class OpenableObject : Interactable
{
    [Header("Sprites")]
    [Tooltip("GameObject atau SpriteRenderer untuk state tertutup")]
    public GameObject closedSprite;
    [Tooltip("GameObject atau SpriteRenderer untuk state terbuka")]
    public GameObject openSprite;

    public GameObject itemInside;

    private bool isOpen = false;

    protected override void Awake()
    {
        base.Awake();
        // Pastikan sprite yang aktif sesuai state awal
        UpdateSpriteState();
        hideItemInside();
    }

    public override void Interact()
    {
        isOpen = !isOpen;
        UpdateSpriteState();
        showItemInside();
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

    public void hideItemInside() {
        if(itemInside != null) {
            itemInside.SetActive(false);
        }
    }

    public void showItemInside() {
        if(itemInside != null) {
            itemInside.SetActive(true);
        }
    }
}
