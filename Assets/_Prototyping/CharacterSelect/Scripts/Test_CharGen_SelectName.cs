using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace DensetsuEngine.Prototyping
{
    public class Test_CharGen_SelectName : MonoBehaviour, IMenuNavigation {

        //[SerializeField] private GameObject rootPanel;
        private SubMenuState _subMenuState = SubMenuState.letterBox;

        [SerializeField] private GameObject rootPanel;

        [SerializeField] private TMP_Text playerNameTextBox;
        [SerializeField] private int playerNameMaxSize = 16;
        [SerializeField] private Image playerCharaImage;
        [SerializeField] private TMP_Text playerClassName;

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

        private enum SubMenuState
        {
            none,
            letterBox,
            nextButton,
            backButton
        }

        private void Awake(){
            rectTransform = selectorIcon.GetComponent<RectTransform>();
            MenuController = FindAnyObjectByType<Test_CharacterSelectMenuController>();

            letterboxGridItems = letterboxGridPanel.GetComponentsInChildren<TMP_Text>();

        }

        private void Update() {
            if (_subMenuState == SubMenuState.letterBox){
                PlayerNameTextBoxFlash();
            }
        }

    
        private void OnLetterboxGridSelect() {
           

            int id = selectorPos.x + (selectorPos.y * selectorMaxX);
            if (id == 59){ // Clear Name
                MenuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.cancel);
                MenuController.playerName = "";
            }
            else if (id == 58){ //Delete last letter
                DeleteLetterInPlayerName();
            }
            else
            {
                //add selected letter to name
                if (MenuController.playerName.Length > playerNameMaxSize)
                {
                    MenuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.cancel);
                    return;
                }

                MenuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.select);
                MenuController.playerName += letterIds[id];
            }


            playerNameTextBox.text = MenuController.playerName;

        }

        private void OnNextButtonSelect() {
            if (MenuController.playerName == "")
            {
                Debug.Log("no player name - please enter name");
                MenuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.cancel);
                return;
            }
            else if (MenuController.playerName.Length < 3)
            {
                Debug.Log("player name must be at least 3 letters");
                MenuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.cancel);
            }
            else {
                Debug.Log("player name accepted");
                MenuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.bigSelect);
                ExitSelectNameMenu();   
            }
        
        }


        private void DeleteLetterInPlayerName() {
            MenuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.cancel);
            if (MenuController.playerName.Length - 1 < 0) return;
            MenuController.playerName = MenuController.playerName.Remove(MenuController.playerName.Length - 1);
            playerNameTextBox.text = MenuController.playerName;
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
            _subMenuState = SubMenuState.letterBox;

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
            _subMenuState = SubMenuState.nextButton;
            nextButtonText.color = Color.red;
        }

        private void ExitNextButton() {
            nextButtonText.color = Color.white;
        }

        private void EnterBackButton() {
            _subMenuState = SubMenuState.backButton;
            backButtonText.color = Color.red;
        }
        private void ExitBackButton() {
            backButtonText.color = Color.white;
        }




        private void ExitSelectNameMenu(){
            _subMenuState = SubMenuState.none;
            StartCoroutine(MenuController.GoToNextMenu());
        }



        #region I Navigation Interface
        public void OnNavUp()
        {
            switch (_subMenuState)
            {
                case SubMenuState.letterBox:
                    selectorPos.y--;
                    UpdateLetterGridSelector();
                    break;
                case SubMenuState.nextButton:
                    selectorPos.y = selectorMaxY - 1;
                    selectorPos.x = selectorMaxX - 1;
                    UpdateLetterGridSelector();
                    ExitNextButton();
                    EnterLetterBox();
                    break;
                case SubMenuState.backButton:
                    selectorPos.y = selectorMaxY - 1;
                    selectorPos.x = 0;
                    UpdateLetterGridSelector();
                    ExitBackButton();
                    EnterLetterBox();
                    break;
            }
        }

        public void OnNavDown()
        {
            switch (_subMenuState)
            {
                case SubMenuState.letterBox:
                    if (selectorPos.y >= selectorMaxY - 1)
                    {
                        ExitLetterBox();
                        if (selectorPos.x < 5)
                        {
                            EnterBackButton();
                        }
                        else
                        {
                            EnterNextButton();
                        }

                    }
                    else
                    {
                        selectorPos.y++;
                        UpdateLetterGridSelector();
                    }
                    break;
                case SubMenuState.nextButton:
                    selectorPos.y = 0;
                    selectorPos.x = selectorMaxX - 1;
                    ExitNextButton();
                    EnterLetterBox();
                    UpdateLetterGridSelector();
                    break;
                case SubMenuState.backButton:
                    break;
            }
        }

        public void OnNavLeft()
        {
            switch (_subMenuState)
            {
                case SubMenuState.letterBox:
                    selectorPos.x--;
                    UpdateLetterGridSelector();
                    break;
                case SubMenuState.nextButton:
                    ExitNextButton();
                    EnterBackButton();
                    break;
                case SubMenuState.backButton:
                    break;
            }
        }

        public void OnNavRight()
        {
            switch (_subMenuState)
            {
                case SubMenuState.letterBox:
                    selectorPos.x++;
                    UpdateLetterGridSelector();
                    break;
                case SubMenuState.nextButton:
                    break;
                case SubMenuState.backButton:
                    ExitBackButton();
                    EnterNextButton();
                    break;
            }
        }

        public void OnNavSelect()
        {
            switch (_subMenuState)
            {
                case SubMenuState.letterBox:
                    OnLetterboxGridSelect();
                    break;
                case SubMenuState.nextButton:
                    OnNextButtonSelect();
                    break;
                case SubMenuState.backButton:
                    StartCoroutine(MenuController.GoToPrevMenu());
                    break;

            }
        }

        public void OnNavCancel()
        {

            switch (_subMenuState)
            {
                case SubMenuState.letterBox:
                    DeleteLetterInPlayerName();
                    break;
                case SubMenuState.nextButton:
                    selectorPos = Vector2Int.zero;
                    UpdateLetterGridSelector();
                    ExitNextButton();
                    EnterLetterBox();
                    break;
                case SubMenuState.backButton:
                    StartCoroutine(MenuController.GoToPrevMenu());
                    break;
            }


             
        }

        public void LoadMenu()
        {
            
            playerCharaImage.sprite = MenuController.GetClassInfo(MenuController.playerClass).classCharaImage_Sq;
            playerClassName.text = "Class: " + MenuController.GetClassInfo(MenuController.playerClass).className;
            
            selectorPos = Vector2Int.zero;
            ExitBackButton();
            ExitNextButton();
            EnterLetterBox();
            Vector2Int startPos = new Vector2Int(24, -24);
            rectTransform.anchoredPosition = new Vector2(startPos.x + (selectorPos.x * moveDisX), startPos.y - +(selectorPos.y * moveDisY));
            _subMenuState = SubMenuState.letterBox;
            rootPanel.gameObject.SetActive(true);
        }

        public void UnloadMenu()
        {
            rootPanel.gameObject.SetActive(false);
        }

        #endregion


    }
}
