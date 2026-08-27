using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class NextAreaController : Interactable
{
    [Header("Next Area Settings")]

    [SerializeField] private Transform destinationPos;
    [SerializeField] private GameObject playerObject;

    public override void Interact()
    {
        GoToNextLevel();
    }

    public void GoToNextLevel()
    {
        playerObject.transform.position = destinationPos.position;
    }
}
