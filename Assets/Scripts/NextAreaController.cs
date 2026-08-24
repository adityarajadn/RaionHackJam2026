using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class NextAreaController : Interactable
{
    [Header("Next Area Settings")]
    [Tooltip("Nama Scene selanjutnya yang akan diload")]
    [SerializeField] private string nextSceneName;

    public override void Interact()
    {
        GoToNextLevel();
    }

    public void GoToNextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            // Reset Time Scale just in case
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next Scene Name belum diisi di Inspector!");
        }
    }
}
