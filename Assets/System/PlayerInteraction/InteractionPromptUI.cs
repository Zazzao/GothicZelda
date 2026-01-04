using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    public static InteractionPromptUI Instance;

    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text verbText;

    private void Awake()
    {
        Instance = this;    
        Hide();
    }

    public void Show(string verb, Vector2 targetPos)
    {
        verbText.text = verb;
        root.SetActive(true);
        root.transform.position = Camera.main.WorldToScreenPoint(targetPos);
    }

    public void Hide()
    {
        root.SetActive(false);
    }

}
