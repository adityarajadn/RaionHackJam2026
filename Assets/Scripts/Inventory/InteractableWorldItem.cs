using UnityEngine;
public class InteractableWorldItem : Interactable
{
    public ItemData itemData;
    [Header("Detection Sprites")]
    public GameObject spriteNotDetected;
    public GameObject spriteDetected;
    protected override void Awake()
    {
        base.Awake();
        if (spriteNotDetected != null) spriteNotDetected.SetActive(true);
        if (spriteDetected != null) spriteDetected.SetActive(false);
    }
    public override void TogglePrompt(bool show)
    {
        base.TogglePrompt(show);
        if (spriteNotDetected != null) spriteNotDetected.SetActive(!show);
        if (spriteDetected != null) spriteDetected.SetActive(show);
    }
    public override void Interact()
    {
        InventoryUI invController = Object.FindFirstObjectByType<InventoryUI>();
        if (invController != null)
        {
            invController.ReceiveWorldItem(this);
            HideWorldItem(); 
        }
    }
    public GameObject GetRootObject()
    {
        return transform.parent != null ? transform.parent.gameObject : gameObject;
    }
    public void HideWorldItem()
    {
        GetRootObject().SetActive(false);
    }
    public void ShowWorldItem()
    {
        GetRootObject().SetActive(true);
    }
    public void DestroyWorldItem()
    {
        Destroy(GetRootObject());
    }
}
