using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



///
/// Logic for the character select concept mockup level
///
namespace DensetsuEngine.Prototyping
{
    public class Test_CharacterSelectMenuController : MonoBehaviour
    {

        [Header("MenuStates")]
        [SerializeField] private MenuState menuState = MenuState.none;


        [Header("PlayerStats")]
        public string playerName = "Hiro";
        [SerializeField] private TMP_Text playerNameTextBox;

        [Header("TitleMenuVar")]
        [SerializeField] private Image[] titleMenuButtons;


        
        [Header("Letter Grid Vars")]
        [SerializeField] private Image selectorIcon;
        [SerializeField] private Vector2Int selectorPos;
        [SerializeField] private int selectorMaxX;
        [SerializeField] private int selectorMaxY;
        [SerializeField] private AudioClip selectorMoveSfx;
        [SerializeField] private AudioClip selectSfx;
        [SerializeField] private AudioClip cancelSfx;


        private char[] letterIds =
            {'A','B','C','D','E','F','G','H','I','J',
            'K','L','M','N','O','P','Q','R','S','T',
            'U','V','W','X','Y','Z','a','b','c','d',
            'e','f','g','h','i','j','k','l','m','n',
            'o','p','q','r','s','t','u','v','w','x',
            'y','z','[',']','-','?','!','_',' ', ' ',
            '0','1','2','3','4','5','6','7','8','9'};

        AudioSource audioSource;

        private PlayerInputActions controls;


        public enum MenuState { 
            none,
            selectClass,
            selectHeirloom,
            selectName,
            confirm
        }

        public enum MenuAudioType { 
            none,
            select,
            cancel,
            move
        
        }

        private Test_CharGen_SelectName selectNameMenu; 


        #region On Enable/Disable
        private void OnEnable()
        {
            controls.Enable();
        }

        private void OnDisable()
        {
            controls.Disable();
        }
        #endregion


        void Awake(){
            audioSource = GetComponent<AudioSource>();

            controls = new PlayerInputActions();

            controls.Menu.Up.performed += OnUpPerformed;
            controls.Menu.Down.performed += OnDownPerformed;
            controls.Menu.Left.performed += OnLeftPerformed;
            controls.Menu.Right.performed += OnRightPerformed;
            controls.Menu.Select.performed += OnSelectPerformed;
            controls.Menu.Cancel.performed += OnCancelPerformed;    

            selectNameMenu = GetComponent<Test_CharGen_SelectName>();




        }

        private void Start(){
           // UpdatePlayerName();
        }



        void Update() {


        }


        private void OnUpPerformed(InputAction.CallbackContext context){
            selectNameMenu.OnSelectorMoveUp();
        }
        private void OnDownPerformed(InputAction.CallbackContext context){
            selectNameMenu.OnSelectorMoveDown();
        }
        private void OnLeftPerformed(InputAction.CallbackContext context){
            selectNameMenu.OnSelectorMoveLeft();
        }
        private void OnRightPerformed(InputAction.CallbackContext context){
            selectNameMenu.OnSelectorMoveRight();

        }

        private void OnSelectPerformed(InputAction.CallbackContext context){

            selectNameMenu.OnSelect();

        }
        private void OnCancelPerformed(InputAction.CallbackContext context){
            selectNameMenu.OnCancel();

        }

        public void UpdatePlayerName() {
            //playerNameTextBox.text = playerName + " _";
            playerNameTextBox.text = playerName;
        }



        public void PlayAudioOneShot(MenuAudioType a) {
            switch (a) {
                case MenuAudioType.select:
                    audioSource.volume = 0.5f;
                    audioSource.PlayOneShot(selectSfx);
                    break;
                case MenuAudioType.cancel:
                    audioSource.volume = 0.35f;
                    audioSource.PlayOneShot(cancelSfx);
                    break;
                case MenuAudioType.move:
                    audioSource.volume = 0.15f;
                    audioSource.PlayOneShot(selectorMoveSfx);
                    break;
                default:
                    break;
            
            }
        }




    }



}
