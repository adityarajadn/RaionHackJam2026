using UnityEngine;

public class NPCInteract : Interactable
{
    [Header("Dialog Settings")]
    public DialogData dialogData;

    public override void Interact()
    {
        if (dialogData != null && DialogManager.Instance != null)
        {
            DialogManager.Instance.StartDialog(dialogData);
        }
        else
        {
            Debug.LogWarning("DialogData atau DialogManager belum diset!");
        }
    }
}
