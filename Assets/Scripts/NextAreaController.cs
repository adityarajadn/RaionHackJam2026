using UnityEngine;
using UnityEngine.SceneManagement;

public class NextAreaController : MonoBehaviour
{
    [Header("UI & Settings")]
    [Tooltip("Masukkan Game Object UI tombol E di sini")]
    [SerializeField] private GameObject promptUI;
    
    [Tooltip("Nama Scene selanjutnya yang akan diload")]
    [SerializeField] private string nextSceneName;

    void Start()
    {
        // Pastikan prompt disembunyikan saat awal mula
        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }

    public void TogglePrompt(bool show)
    {
        if (promptUI != null)
        {
            promptUI.SetActive(show);
        }
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
