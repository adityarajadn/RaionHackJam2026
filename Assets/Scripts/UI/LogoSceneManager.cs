using UnityEngine;

public class LogoSceneManager : MonoBehaviour
{
    // Dipanggil dari Animation Event untuk pindah ke Main Menu
    public void GoToMainMenu()
    {
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.LoadScene("MainMenu");
        }
        else
        {
            // Fallback kalau GameSceneManager belum ada di scene ini
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

    // Alternatif jika ingin pindah ke scene spesifik dari Inspector / Animation Event
    public void LoadScene(string sceneName)
    {
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.LoadScene(sceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}
