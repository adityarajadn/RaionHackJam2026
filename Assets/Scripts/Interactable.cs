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
        // Pastikan collider diset sebagai trigger
        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null)
        {
            coll.isTrigger = true;
        }

        // Otomatis assign tag jika belum diset
        if (gameObject.CompareTag("Untagged"))
        {
            gameObject.tag = "Interactable";
        }

        // Sembunyikan prompt pada awalnya
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

    // Fungsi utama yang akan dioverride oleh script-script turunannya
    public abstract void Interact();
}
