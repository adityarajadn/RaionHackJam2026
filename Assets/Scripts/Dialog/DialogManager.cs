using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public float autoContinueDelay = 0.8f;

    private DialogData currentDialog;
    private int currentSentenceIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private Coroutine autoContinueCoroutine;

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
        dialogPanel.SetActive(false);
    }

    private void Update()
    {
        // Pakai New Input System (cek spasi)
        if (dialogPanel.activeSelf && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            DisplayNextSentence();
        }
    }

    public void StartDialog(DialogData dialogData)
    {
        currentDialog = dialogData;
        currentSentenceIndex = 0;
        dialogPanel.SetActive(true);
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        // Hentikan coroutine auto-continue jika ada
        if (autoContinueCoroutine != null)
        {
            StopCoroutine(autoContinueCoroutine);
            autoContinueCoroutine = null;
        }

        if (isTyping)
        {
            // Skip typing and show full sentence
            CompleteTyping();
            return;
        }

        if (currentSentenceIndex < currentDialog.sentences.Length)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeSentence(currentDialog.sentences[currentSentenceIndex]));
            currentSentenceIndex++;
        }
        else
        {
            EndDialog();
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogText.text = "";
        isTyping = true;
        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;

        // Auto lanjut setelah sekian detik
        autoContinueCoroutine = StartCoroutine(AutoContinueWait());
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        // Tampilkan kalimat utuh
        dialogText.text = currentDialog.sentences[currentSentenceIndex - 1];
        isTyping = false;

        // Jika diskip, tetap kasih delay sebelum kalimat selanjutnya
        autoContinueCoroutine = StartCoroutine(AutoContinueWait());
    }

    private IEnumerator AutoContinueWait()
    {
        yield return new WaitForSeconds(autoContinueDelay);
        DisplayNextSentence();
    }

    private void EndDialog()
    {
        dialogPanel.SetActive(false);
    }
}

