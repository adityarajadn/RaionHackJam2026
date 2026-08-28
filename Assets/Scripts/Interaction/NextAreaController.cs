using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
public class NextAreaController : Interactable
{
    [Header("Next Area Settings")]
    [SerializeField] private Transform destinationPos;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private string transitionSFX;
    public override void Interact()
    {
        GoToNextLevel();
    }
    public void GoToNextLevel()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("door");
        }
        if (playerObject != null && destinationPos != null)
        {
            playerObject.transform.position = destinationPos.position;
        }
    }
}
