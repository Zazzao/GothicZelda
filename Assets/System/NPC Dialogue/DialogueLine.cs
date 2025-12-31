using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public Sprite portrait;
    [TextArea(3, 5)]
    public string message;
    public DialogueSide side; // Left or Right
}

public enum DialogueSide
{
    Left,
    Right
}