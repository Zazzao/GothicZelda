using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


namespace DensetsuEngine.Prototyping
{
    public class Test_CharGen_SelectName : MonoBehaviour {


        private MenuState _menuState = MenuState.letterBox;


        [SerializeField] private TMP_Text playerNameTextBox;
        [SerializeField] private int playerNameMaxSize = 16;

        [Header("Letter Box Selector")]
        [SerializeField] private Image selectorIcon;
        [SerializeField] private Vector2Int selectorPos;
        [SerializeField] private int selectorMaxX;
        [SerializeField] private int selectorMaxY;

        [Header("Letter Box Grid")]
        [SerializeField] GameObject letterboxGridPanel;
        TMP_Text[] letterboxGridItems;

        [SerializeField] private Color letterboxGridColor_active;
        [SerializeField] private Color letterboxGridColor_inactive;
        [SerializeField] private Color greyoutletterColor;
        [SerializeField] private Color letterColor;


        [Header("Text Indicator")]
        [SerializeField] private string blankLetterFlashColor = "940014";
        [SerializeField] private float letterFlashTime = 0.25f;
        [SerializeField] private float letterFlashTimer = 0.0f;
        private bool letterFlashOn = true;


        [Header("Next Button")]
        [SerializeField] private TMP_Text nextButtonText;
        [SerializeField] private TMP_Text backButtonText;



        Test_CharacterSelectMenuController MenuController;
        private RectTransform rectTransform;
        private int moveDisX = 25;
        private int moveDisY = 20;

        private char[] letterIds =
           {'A','B','C','D','E','F','G','H','I','J',
            'K','L','M','N','O','P','Q','R','S','T',
            'U','V','W','X','Y','Z','a','b','c','d',
            'e','f','g','h','i','j','k','l','m','n',
            'o','p','q','r','s','t','u','v','w','x',
            'y','z','[',']','-','?','!',' ',' ', ' ',
            '0','1','2','3','4','5','6','7','8','9'};

        private enum MenuState
        {
            letterBox,
            nextButton,
            backButton
        }

        private void Awake()
        {
            rectTransform = selectorIcon.GetComponent<RectTransform>();
            MenuController = GetComponent<Test_CharacterSelectMenuController>();

            letterboxGridItems = letterboxGridPanel.GetComponentsInChildren<TMP_Text>();

        }

        private void Start()
        {
            EnterLetterBox();
        }

        private void Update() {
            if (_menuState == MenuState.letterBox)
            {
                PlayerNameTextBoxFlash();
            }
        }



        public void OnSelectorMoveUp() {
            switch (_menuState) {
                case MenuState.letterBox:
                    selectorPos.y--;
                    UpdateLetterGridSelector();
                    break;
                case MenuState.nextButton:
                    selectorPos.y = selectorMaxY - 1;
                    selectorPos.x = selectorMaxX - 1;
                    UpdateLetterGridSelector();
                    ExitNextButton();
                    EnterLetterBox();
                    break;
                case  MenuState.backButton:
                    selectorPos.y = selectorMaxY - 1;
                    selectorPos.x = 0;
                    UpdateLetterGridSelector();
                    ExitBackButton();
                    EnterLetterBox();
                    break;
            }

        }

        public void OnSelectorMoveDown()
        {
            switch (_menuState) {
                case MenuState.letterBox:
                    if (selectorPos.y >= selectorMaxY - 1) {
                        ExitLetterBox();
                        if (selectorPos.x < 5) {
                            EnterBackButton();
                        }
                        else{ 
                            EnterNextButton();
                        }
                           
                    }
                    else {
                        selectorPos.y++;
                        UpdateLetterGridSelector();
                    }
                    break;
                case MenuState.nextButton:
                    selectorPos.y = 0;
                    selectorPos.x = selectorMaxX-1;
                    ExitNextButton();
                    EnterLetterBox();
                    UpdateLetterGridSelector();
                    break;
                    case MenuState.backButton:
                break;
            }
        }

        public void OnSelectorMoveLeft()
        {
            switch (_menuState) {
                case MenuState.letterBox:
                    selectorPos.x--;
                    UpdateLetterGridSelector();
                    break;
                case MenuState.nextButton:
                    ExitNextButton();
                    EnterBackButton();  
                    break;
                case MenuState.backButton:
                    break;
            }

        }
        public void OnSelectorMoveRight()
        {
            switch (_menuState)
            {
                case MenuState.letterBox:
                    selectorPos.x++;
                    UpdateLetterGridSelector();
                    break;
                case MenuState.nextButton:
                    break;
                case MenuState.backButton:
                    ExitBackButton();
                    EnterNextButton();
                    break;
            }



        }

        public void OnSelect() {

            switch (_menuState) {
                case MenuState.letterBox:
                    OnLetterboxGridSelect();
                    break;
                case MenuState.nextButton:
                    break;
                case MenuState.backButton:
                    break;

            }


        }


        private void OnLetterboxGridSelect() {
            if (MenuController.playerName.Length > playerNameMaxSize)
            {
                MenuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.cancel);
                return;
            }

            int id = selectorPos.x + (selectorPos.y * selectorMaxX);
            //NOTE: id 59 = clear name
            //NOTE: id 58 = delete letter
            if (id == 59)
            {
                MenuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.cancel);
                MenuController.playerName = "";
            }
            else if (id == 58)
            {
                DeleteLetterInPlayerName();
            }
            else
            {
                MenuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.select);
                MenuController.playerName += letterIds[id];
            }



            MenuController.UpdatePlayerName();

        }




        public void OnCancel() {
            if (_menuState == MenuState.letterBox) DeleteLetterInPlayerName();
        }



        private void DeleteLetterInPlayerName() {
            MenuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.cancel);
            if (MenuController.playerName.Length - 1 < 0) return;
            MenuController.playerName = MenuController.playerName.Remove(MenuController.playerName.Length - 1);
            MenuController.UpdatePlayerName();
        }

        private void UpdateLetterGridSelector() {

            MenuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.move);

            if (selectorPos.x >= selectorMaxX) {
                selectorPos.x = 0;
            }
            else if (selectorPos.x < 0) {
                selectorPos.x = selectorMaxX - 1;
            }

            if (selectorPos.y >= selectorMaxY) {
                selectorPos.y = 0;
            }
            else if (selectorPos.y < 0) {
                selectorPos.y = selectorMaxY - 1;
            }


            Vector2Int startPos = new Vector2Int(24, -24);
            rectTransform.anchoredPosition = new Vector2(startPos.x + (selectorPos.x * moveDisX), startPos.y - +(selectorPos.y * moveDisY));

        }


        private void PlayerNameTextBoxFlash() {
            //dont flash if player name too long
            if (MenuController.playerName.Length > playerNameMaxSize)
            {
                playerNameTextBox.text = MenuController.playerName;
                return;
            }

            letterFlashTimer -= Time.deltaTime;
            if (letterFlashTimer < 0)
            {
                if (letterFlashOn)
                {
                    playerNameTextBox.text = MenuController.playerName + "_";
                }
                else
                {
                    playerNameTextBox.text = MenuController.playerName + "<color=#" + blankLetterFlashColor + ">_</color>";
                }
                letterFlashTimer = letterFlashTime;
                letterFlashOn = !letterFlashOn;
            }

        }



        private void EnterLetterBox() {

            letterboxGridPanel.GetComponent<Image>().color = letterboxGridColor_active;
            foreach (TMP_Text letter in letterboxGridItems) {
                letter.color = letterColor;
            }

            selectorIcon.enabled = true;
            _menuState = MenuState.letterBox;

        }

        private void ExitLetterBox() {
            selectorIcon.enabled = false;
            letterboxGridPanel.GetComponent<Image>().color = letterboxGridColor_inactive;
            foreach (TMP_Text letter in letterboxGridItems) {
                letter.color = greyoutletterColor;
            }
            playerNameTextBox.text = MenuController.playerName;
        }


        private void EnterNextButton() {
            _menuState = MenuState.nextButton;
            nextButtonText.color = Color.red;
        }

        private void ExitNextButton() {
            nextButtonText.color = Color.white;
        }

        private void EnterBackButton() {
            _menuState = MenuState.backButton;
            backButtonText.color = Color.red;
        }
        private void ExitBackButton() {
            backButtonText.color = Color.white;
        }

    }
}
