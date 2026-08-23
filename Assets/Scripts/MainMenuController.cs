using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        // Pindah ke scene TestScene
        SceneManager.LoadScene("TestScene");
    }

    public void OpenSettings()
    {
        Debug.Log("Scene Setting dibuka!");
    }

    public void ExitGame()
    {
        Debug.Log("Keluar dari Game!");
        Application.Quit();
    }
}
