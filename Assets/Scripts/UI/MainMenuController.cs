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
        GameSceneManager.Instance.LoadScene("HowToPlay");
    }
    public void OpenCredits()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("clickButton");
        GameSceneManager.Instance.LoadScene("Credit");
    }
    public void OpenSettings() {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("clickButton");
        GameSceneManager.Instance.LoadScene("Settings");
    }
    public void ExitGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("clickButton");
        Application.Quit();
    }
}
