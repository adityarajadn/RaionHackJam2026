using UnityEngine;

public class InteractableWorldItem : Interactable
{
    public ItemData itemData;
    
    // Awake dan TogglePrompt sudah di-handle oleh base class (Interactable)

    public override void Interact()
    {
        InventoryController invController = Object.FindFirstObjectByType<InventoryController>();
        if (invController != null)
        {
            invController.ReceiveWorldItem(this);
            HideWorldItem(); // Sembunyikan SATU KESATUAN item dari dunia
        }
    }

    public GameObject GetRootObject()
    {
        // Jika parent ada, asumsikan parent adalah root dari item ini (seperti TestItem yang memiliki child DetectionArea)
        // Jika tidak, gunakan diri sendiri.
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
