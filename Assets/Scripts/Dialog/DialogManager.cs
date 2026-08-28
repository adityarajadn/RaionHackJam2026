using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }
    [Header("UI Elements")]
    [FormerlySerializedAs("dialogPanel")]
    [SerializeField] private GameObject _dialogPanel;
    [FormerlySerializedAs("dialogText")]
    [SerializeField] private TextMeshProUGUI _dialogText;
    [Header("Settings")]
    [FormerlySerializedAs("typingSpeed")]
    [SerializeField] private float _typingSpeed = 0.05f;
    [FormerlySerializedAs("autoContinueDelay")]
    [SerializeField] private float _autoContinueDelay = 0.8f;
    private DialogData _currentDialog;
    private int _currentSentenceIndex = 0;
    private bool _isTyping = false;
    private Coroutine _typingCoroutine;
    private Coroutine _autoContinueCoroutine;
    public bool IsDialogActive => _dialogPanel != null && _dialogPanel.activeSelf;
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
        _dialogPanel.SetActive(false);
    }
    private void Update()
    {
        if (_dialogPanel.activeSelf && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            DisplayNextSentence();
        }
    }
    public void StartDialog(DialogData dialogData)
    {
        _currentDialog = dialogData;
        _currentSentenceIndex = 0;
        Time.timeScale = 0f; 
        _dialogPanel.SetActive(true);
        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        if (_autoContinueCoroutine != null)
        {
            StopCoroutine(_autoContinueCoroutine);
            _autoContinueCoroutine = null;
        }
        if (_isTyping)
        {
            CompleteTyping();
            return;
        }
        if (_currentSentenceIndex < _currentDialog.sentences.Length)
        {
            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
            }
            _typingCoroutine = StartCoroutine(TypeSentence(_currentDialog.sentences[_currentSentenceIndex]));
            _currentSentenceIndex++;
        }
        else
        {
            EndDialog();
        }
    }
    private IEnumerator TypeSentence(string sentence)
    {
        _dialogText.text = "";
        _isTyping = true;
        foreach (char letter in sentence.ToCharArray())
        {
            _dialogText.text += letter;
            yield return new WaitForSecondsRealtime(_typingSpeed);
        }
        _isTyping = false;
        _autoContinueCoroutine = StartCoroutine(AutoContinueWait());
    }
    private void CompleteTyping()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }
        _dialogText.text = _currentDialog.sentences[_currentSentenceIndex - 1];
        _isTyping = false;
        _autoContinueCoroutine = StartCoroutine(AutoContinueWait());
    }
    private IEnumerator AutoContinueWait()
    {
        yield return new WaitForSecondsRealtime(_autoContinueDelay);
        DisplayNextSentence();
    }
    private void EndDialog()
    {
        _dialogPanel.SetActive(false);
        if (GameplayManager.Instance == null || !GameplayManager.Instance.IsGameOver)
        {
            Time.timeScale = 1f; 
        }
    }
}
