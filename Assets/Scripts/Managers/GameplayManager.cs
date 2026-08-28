using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }
    [Header("Game State")]
    [FormerlySerializedAs("totalScore")]
    [SerializeField] private int _totalScore = 0;
    [FormerlySerializedAs("totalWeight")]
    [SerializeField] private float _totalWeight = 0f;
    [FormerlySerializedAs("maxWeight")]
    [SerializeField] private float _maxWeight = 100f; 
    private bool _isGameOver = false;
    public int TotalScore 
    { 
        get => _totalScore; 
        set => _totalScore = value; 
    }
    public float TotalWeight 
    { 
        get => _totalWeight; 
        set => _totalWeight = value; 
    }
    public float MaxWeight => _maxWeight;
    public bool IsGameOver => _isGameOver;
    [Header("End Game UI")]
    [Tooltip("Panel UI yang akan muncul saat waktu habis")]
    [FormerlySerializedAs("endGamePanel")]
    [SerializeField] private GameObject _endGamePanel;
    [Tooltip("Teks untuk menampilkan skor akhir di panel")]
    [FormerlySerializedAs("finalScoreText")]
    [SerializeField] private TextMeshProUGUI _finalScoreText;
    [Tooltip("Teks untuk menampilkan total berat akhir di panel")]
    [FormerlySerializedAs("finalWeightText")]
    [SerializeField] private TextMeshProUGUI _finalWeightText;
    private void Awake()
    {
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
        if (_endGamePanel != null)
        {
            _endGamePanel.SetActive(false);
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic("Ingame");
        }
    }
    public void AddScore(int amount)
    {
        if (_isGameOver) return; 
        _totalScore += amount;
    }
    public void OnTimeUp()
    {
        if (_isGameOver) return;
        _isGameOver = true;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("result");
        }
        Time.timeScale = 0f;
        int highestScore = PlayerPrefs.GetInt("HighestScore", 0);
        if (_totalScore > highestScore)
        {
            highestScore = _totalScore;
            PlayerPrefs.SetInt("HighestScore", highestScore);
            PlayerPrefs.Save();
        }
        if (_endGamePanel != null)
        {
            _endGamePanel.SetActive(true);
        }
        if (_finalScoreText != null)
        {
            _finalScoreText.text = "Total Value: " + _totalScore.ToString();
        }
        if (_finalWeightText != null)
        {
            _finalWeightText.text = "Total Weight: " + _totalWeight.ToString("F1") + " kg";
        }
    }
    public void RestartLevel()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("clickButton");
        Time.timeScale = 1f;
        GameSceneManager.Instance.ReloadScene();
    }
    public void ReturnToMainMenu(string mainMenuSceneName = "MainMenu")
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("clickButton");
        Time.timeScale = 1f;
        GameSceneManager.Instance.LoadScene(mainMenuSceneName);
    }
}
