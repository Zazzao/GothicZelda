using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;


public class DialogueUI : MonoBehaviour
{

    public static DialogueUI Instance;

    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI[] nameText;
    [SerializeField] private TextMeshProUGUI[] titleText;
    [SerializeField] private TextMeshProUGUI[] dialogueText;
    [SerializeField] private Image[] portraitImage;
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;

    [Header("Text Settings")]
    [SerializeField] private float textTypingSpeed = 0.03f;
    [SerializeField] private int typingSfxRate = 4;
    private int boxIndex = 0;

    private Coroutine typingRoutine;
    public bool IsTyping { get; private set; }

    [Header("Sfx")]
    [SerializeField] private AudioClip textTypingSfx;
    [SerializeField] private AudioClip textSkipSfx;


    private AudioSource audioSource;
   


    private void Awake()
    {
        Instance = this;
        root.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    public void Show() => root.SetActive(true);
    public void Hide() => root.SetActive(false);

    public void DisplayLine(DialogueLine line){

        if (typingRoutine != null) StopCoroutine(typingRoutine);

        if (line.side == DialogueSide.Right)
            boxIndex = 0;
        else
            boxIndex = 1;

        nameText[boxIndex].text = line.speakerName;
        titleText[boxIndex].text = line.title;
        portraitImage[boxIndex].sprite = line.portrait;

        leftPanel.SetActive(line.side == DialogueSide.Left);
        rightPanel.SetActive(line.side == DialogueSide.Right);

        typingRoutine = StartCoroutine(TypeText(line.message));
    }

    private IEnumerator TypeText(string message){
        IsTyping = true;
        var text = dialogueText[boxIndex];

        text.text = message;
        text.maxVisibleCharacters = 0;

        for (int i = 0; i <= message.Length; i++){
            text.maxVisibleCharacters = i;

            if (i % typingSfxRate == 0) audioSource.PlayOneShot(textTypingSfx);

            yield return new WaitForSeconds(textTypingSpeed);
        }

        IsTyping = false;
    }

    public void SkipTyping(string message){
        if (!IsTyping) return;

        audioSource.PlayOneShot(textSkipSfx);
        StopCoroutine(typingRoutine);
        dialogueText[boxIndex].maxVisibleCharacters = dialogueText[boxIndex].text.Length;
        IsTyping = false;
    }


}
