using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private DialogueData currentDialogue;
    private int currentIndex;
    private bool isDialogueActive;

    private void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(DialogueData dialogue)
    {
        if (isDialogueActive) return;

        isDialogueActive = true;
        currentDialogue = dialogue;
        currentIndex = 0;

        PlayerMovement.instance.IsFrozen = true;
        DialogueUI.Instance.Show();

        ShowCurrentLine();
    }

    public void AdvanceDialogue()
    {
        if (DialogueUI.Instance.IsTyping)
        {
            DialogueUI.Instance.SkipTyping();
            return;
        }

        currentIndex++;

        if (currentIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
        }
        else
        {
            ShowCurrentLine();
        }
    }

    private void ShowCurrentLine()
    {
        DialogueUI.Instance.DisplayLine(currentDialogue.lines[currentIndex]);
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        DialogueUI.Instance.Hide();
        PlayerMovement.instance.IsFrozen = false;
    }
}




