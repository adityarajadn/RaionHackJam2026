using UnityEngine;
using UnityEngine.Serialization;
public class NPCInteract : Interactable
{
    [Header("Dialog Settings")]
    [FormerlySerializedAs("dialogData")]
    [SerializeField] private DialogData _dialogData;
    public override void Interact()
    {
        if (_dialogData != null && DialogManager.Instance != null)
        {
            DialogManager.Instance.StartDialog(_dialogData);
        }
        else
        {
        }
    }
}
