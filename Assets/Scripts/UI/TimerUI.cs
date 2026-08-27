using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [Tooltip("The TextMeshProUGUI component to display the time")]
    public TextMeshProUGUI timerText;

    [Tooltip("Set waktu mulai timer di sini (dalam hitungan detik)")]
    public float initialTimeInSeconds = 60f;

    private float currentTime = 0f;
    private bool isTimerRunning = false;

    void Start()
    {
        if (timerText == null)
        {
            timerText = GetComponent<TextMeshProUGUI>();
        }
        currentTime = initialTimeInSeconds;
        isTimerRunning = true;
    }

    void Update()
    {
        if (isTimerRunning)
        {
            float delta = Time.deltaTime;
            // Jika inventory buka (timeScale = 0), tetap hitung timer pakai unscaledDeltaTime
            if (InventoryController.IsInventoryOpen && Time.timeScale == 0f)
            {
                delta = Time.unscaledDeltaTime;
            }

            // Tapi kalau game over, jangan dikurangi
            if (GameplayManager.Instance != null && GameplayManager.Instance.isGameOver)
            {
                delta = 0f;
            }

            currentTime -= delta;
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                isTimerRunning = false;
                
                // Beritahu GameplayManager bahwa waktu telah habis
                if (GameplayManager.Instance != null)
                {
                    GameplayManager.Instance.OnTimeUp();
                }
            }
            UpdateTimerDisplay(currentTime);
        }
    }

    private void UpdateTimerDisplay(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        int milliseconds = Mathf.FloorToInt((timeToDisplay % 1) * 1000);

        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
    }

    public void StartTimer()
    {
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = initialTimeInSeconds;
    }
}