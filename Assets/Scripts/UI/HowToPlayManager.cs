using UnityEngine;
using UnityEngine.SceneManagement;
public class HowToPlayManager : MonoBehaviour
{
    public void LoadRoom1()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("buka pintu&pindah ruangan");
        GameSceneManager.Instance.LoadScene("Room1");
    }
}
