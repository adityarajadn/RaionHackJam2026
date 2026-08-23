using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditController : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
