using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [Header("Game State")]
    public int totalScore = 0;
    public bool isGameOver = false;

    [Header("End Game UI")]
    [Tooltip("Panel UI yang akan muncul saat waktu habis")]
    public GameObject endGamePanel;
    [Tooltip("Teks untuk menampilkan skor akhir di panel")]
    public TextMeshProUGUI finalScoreText;

    private void Awake()
    {
        // Singleton pattern sederhana agar bisa diakses dari script mana saja dengan GameplayManager.Instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Pastikan panel akhir game disembunyikan saat mulai
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }
    }

    // Fungsi untuk menambah skor (misalnya saat player mengambil barang berharga)
    public void AddScore(int amount)
    {
        if (isGameOver) return; // Jangan tambah skor kalau game sudah selesai

        totalScore += amount;
        Debug.Log("Skor bertambah! Total skor saat ini: " + totalScore);
        
        // (Opsional) Kamu bisa update teks UI skor in-game di sini
    }

    // Fungsi yang dipanggil ketika waktu habis
    public void OnTimeUp()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("Waktu habis! Game Over.");

        // Pause game
        Time.timeScale = 0f;

        // Cek dan simpan Highest Score
        int highestScore = PlayerPrefs.GetInt("HighestScore", 0);
        if (totalScore > highestScore)
        {
            highestScore = totalScore;
            PlayerPrefs.SetInt("HighestScore", highestScore);
            PlayerPrefs.Save();
        }

        // Tampilkan UI Score
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Total Value: " + totalScore.ToString() + "\nHighest Value: " + highestScore.ToString();
        }
    }

    // Fungsi tambahan untuk tombol di UI (misalnya tombol Restart atau Main Menu)
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu(string mainMenuSceneName = "MainMenu")
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
