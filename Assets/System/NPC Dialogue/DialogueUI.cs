using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class DialogueUI : MonoBehaviour
{

    public static DialogueUI Instance;

    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;

    private Coroutine typingRoutine;
    public bool IsTyping { get; private set; }

    private void Awake()
    {
        Instance = this;
        root.SetActive(false);
    }

    public void Show() => root.SetActive(true);
    public void Hide() => root.SetActive(false);

    public void DisplayLine(DialogueLine line)
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        nameText.text = line.speakerName;
        portraitImage.sprite = line.portrait;

        //leftPanel.SetActive(line.side == DialogueSide.Left);
        //rightPanel.SetActive(line.side == DialogueSide.Right);

        typingRoutine = StartCoroutine(TypeText(line.message));
    }

    private IEnumerator TypeText(string message)
    {
        IsTyping = true;
        dialogueText.text = "";

       string wrappedText = PreWrapText(message,dialogueText, dialogueText.rectTransform.rect.width);

        foreach (char c in wrappedText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        IsTyping = false;
    }

    public void SkipTyping(string message)
    {
        if (!IsTyping) return;

        StopCoroutine(typingRoutine);
        dialogueText.text = message;
        IsTyping = false;
    }


    private string PreWrapText(
        string rawText,
        TextMeshProUGUI tmpText,
        float maxLineWidth)
    {
        tmpText.enableWordWrapping = false;

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

        return result.ToString();
    }








}
