using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Text;

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
    [SerializeField] private int typingSfxRate = 6;
    private int boxIndex = 0;

    private Coroutine typingRoutine;
    public bool IsTyping { get; private set; }

    [Header("Sfx")]
    [SerializeField] private AudioClip textTypingSfx;


    private AudioSource audioSource;
    private int typingTextCnt = 0;



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
        dialogueText[boxIndex].text = "";

        string wrappedText = PreWrapText(message,dialogueText[boxIndex], dialogueText[boxIndex].rectTransform.rect.width);

        foreach (char c in wrappedText){
            dialogueText[boxIndex].text += c;
            yield return new WaitForSeconds(textTypingSpeed);
            typingTextCnt++;
            if (typingTextCnt >= typingSfxRate) audioSource.PlayOneShot(textTypingSfx); //TO-DO: dont use a specific number here
        }

        IsTyping = false;
    }

    public void SkipTyping(string message){
        if (!IsTyping) return;

        StopCoroutine(typingRoutine);
        dialogueText[boxIndex].text = message;
        IsTyping = false;
    }


    private string PreWrapText(string rawText, TextMeshProUGUI tmpText, float maxLineWidth)
    {
        //tmpText.enableWordWrapping = false;

        string[] words = rawText.Split(' ');
        StringBuilder result = new StringBuilder();

        string currentLine = "";

        foreach (string word in words)
        {
            string testLine = string.IsNullOrEmpty(currentLine)
                ? word
                : currentLine + " " + word;

            tmpText.text = testLine;
            tmpText.ForceMeshUpdate();

            float textWidth = tmpText.preferredWidth;

            if (textWidth > maxLineWidth)
            {
                // Commit the current line and start a new one
                result.Append(currentLine);
                result.Append('\n');
                currentLine = word;
            }
            else
            {
                currentLine = testLine;
            }
        }

        // Append last line
        result.Append(currentLine);
        tmpText.text = "";

        return result.ToString();
    }








}
