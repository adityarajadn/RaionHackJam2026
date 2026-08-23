using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider musicSlider;
    public Slider sfxSlider;
    private bool isPaused = false;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            if (musicSlider != null)
            {
                musicSlider.value = AudioManager.Instance.GetMusicVolume();
                musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
            }
            if (sfxSlider != null)
            {
                sfxSlider.value = AudioManager.Instance.GetSFXVolume();
                sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
            }
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleSettings();
        }
    }

    public void ToggleSettings()
    {
        isPaused = !isPaused;
        settingsPanel.SetActive(isPaused);
        
        // Pause time if setting panel is open
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // Jangan lupa kembalikan waktu agar scene lain tidak ikut terhenti
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
