using UnityEngine;
using UnityEngine;
using TMPro;

public class SignUI : MonoBehaviour
{

    public static SignUI Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI text;

    public bool IsOpen => panel.activeSelf;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }


    public void Show(string message)
    {
        text.text = message;
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

}
