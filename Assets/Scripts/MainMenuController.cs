using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        // Pindah ke scene TestScene
        SceneManager.LoadScene("TestScene");
        AudioManager.Instance.PlayMusic("SneakyMusic");
    }

    public void OpenSettings()
    {
        Debug.Log("Scene Setting dibuka!");
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credit");
    }

    public void ExitGame()
    {
        Debug.Log("Keluar dari Game!");
        Application.Quit();
    }
}
