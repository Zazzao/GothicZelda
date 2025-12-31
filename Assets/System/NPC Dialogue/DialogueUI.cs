using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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
    }

    public void Show() => root.SetActive(true);
    public void Hide() => root.SetActive(false);

    public void DisplayLine(DialogueLine line)
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        nameText.text = line.speakerName;
        portraitImage.sprite = line.portrait;

        leftPanel.SetActive(line.side == DialogueSide.Left);
        rightPanel.SetActive(line.side == DialogueSide.Right);

        typingRoutine = StartCoroutine(TypeText(line.message));
    }

    private IEnumerator TypeText(string message)
    {
        IsTyping = true;
        dialogueText.text = "";

        foreach (char c in message)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        IsTyping = false;
    }

    public void SkipTyping()
    {
        if (!IsTyping) return;

        StopCoroutine(typingRoutine);
       // dialogueText.text = currentDialogueLine.message;
        IsTyping = false;
    }


}
