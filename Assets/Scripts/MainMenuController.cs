using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic("MainMenu");
        }
    }
    public void PlayGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("clickButton");
        // Pindah ke scene TestScene
        SceneManager.LoadScene("HowToPlay");
    }

    public void OpenCredits()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("clickButton");
        SceneManager.LoadScene("Credit");
    }

    public void OpenSettings() {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("clickButton");
        SceneManager.LoadScene("Settings");
    }

    public void ExitGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("clickButton");
        Debug.Log("Keluar dari Game!");
        Application.Quit();
    }
}
