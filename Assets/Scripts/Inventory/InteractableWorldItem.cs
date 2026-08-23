using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractableWorldItem : MonoBehaviour
{
    public ItemData itemData;
    
    [Tooltip("Objek visual 'Key E' yang akan muncul saat player mendekat")]
    public GameObject keyEPrompt;

    private void Awake()
    {
        // Pastikan collider diset sebagai trigger
        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null)
        {
            coll.isTrigger = true;
        }
        
        // Pastikan tag-nya "Interactable" sesuai dengan PlayerController
        gameObject.tag = "Interactable";

        // Sembunyikan prompt pada awalnya
        if (keyEPrompt != null)
        {
            keyEPrompt.SetActive(false);
        }
    }

    public void TogglePrompt(bool show)
    {
        if (keyEPrompt != null)
        {
            keyEPrompt.SetActive(show);
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
