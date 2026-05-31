using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class AddItem_UI : MonoBehaviour
{
    
    public static AddItem_UI Instance;


    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image itemIcon;

    public bool IsOpen => panel.activeSelf;

    void Awake(){
        Instance = this;
        panel.SetActive(false);
    }



    public void Show(string itemName, string amount, Sprite itemIcon = null) {
        text.text = "Recieved " + itemName + " " + "x" + amount;

        if (itemIcon != null){
            this.itemIcon.enabled = true;
            this.itemIcon.sprite = itemIcon;
        }
        else {
            this.itemIcon.enabled = false;
        }
        panel.SetActive(true);
    }

    public void Hide() {
        panel.SetActive(false);
    }
    
}
