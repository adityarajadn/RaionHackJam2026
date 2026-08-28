using UnityEngine;
using UnityEngine.Serialization;
[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour
{
    [Tooltip("Objek visual 'Key E' yang akan muncul saat player mendekat")]
    [FormerlySerializedAs("promptUI")]
    public GameObject keyEPrompt;
    protected virtual void Awake()
    {
        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null)
        {
            coll.isTrigger = true;
        }
        if (gameObject.CompareTag("Untagged"))
        {
            gameObject.tag = "Interactable";
        }
        if (keyEPrompt != null)
        {
            keyEPrompt.SetActive(false);
        }
    }
    public virtual void TogglePrompt(bool show)
    {
        if (keyEPrompt != null)
        {
            keyEPrompt.SetActive(show);
        }
    }
    public abstract void Interact();
}
