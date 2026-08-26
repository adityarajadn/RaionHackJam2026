using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        // Pindah ke scene TestScene
        SceneManager.LoadScene("HowToPlay");
        AudioManager.Instance.PlayMusic("SneakyMusic");
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credit");
    }

    public void OpenSettings() {
        SceneManager.LoadScene("Settings");
    }

    public void ExitGame()
    {
        Debug.Log("Keluar dari Game!");
        Application.Quit();
    }
}
