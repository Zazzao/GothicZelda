using System.Collections;
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
        //[SerializeField] private MenuState menuState = MenuState.none;
        [SerializeField] private GameObject[] MenuPanels;

        [SerializeField] private CanvasGroup blkOverlay;
        [SerializeField] private float fadeDuration = 0.35f;


        [Header("PlayerStats")]
        public string playerName = "Hiro";
        [SerializeField] private TMP_Text playerNameTextBox;
        //[SerializeField] public Sprite playerCharaImage;
        [SerializeField] private int playerHp;
        [SerializeField] private int playerMp;
        [SerializeField] private int playerSp;
        [SerializeField] public int playerClass;
        [SerializeField] public int playerHeirloom;



        [Header("Menu Sfx")]
        [SerializeField] private AudioClip selectorMoveSfx;
        [SerializeField] private AudioClip selectSfx;
        [SerializeField] private AudioClip bigSelectSfx;
        [SerializeField] private AudioClip cancelSfx;
        private AudioSource audioSource;


        [Header("I navigation list")]
        [SerializeField] MonoBehaviour[] menuNavigations;
        private int menuNavIndex = 0;
        private IMenuNavigation curMenuNav;


        private PlayerInputActions controls;

        [Header("Game Data")]
        [SerializeField] private StartingClassInfo[] startingClasses;
        [SerializeField] private HeirloomItemInfo[] startingHeirlooms;

        private bool canNavigateMenu = true;

        public enum MenuState {
            none,
            title,
            selectClass,
            selectHeirloom,
            selectName,
            confirm
        }

        public enum MenuAudioType {
            none,
            select,
            bigSelect,
            cancel,
            move

        }


        



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


        void Awake() {
            audioSource = GetComponent<AudioSource>();

            controls = new PlayerInputActions();

            controls.Menu.Up.performed += OnUpPerformed;
            controls.Menu.Down.performed += OnDownPerformed;
            controls.Menu.Left.performed += OnLeftPerformed;
            controls.Menu.Right.performed += OnRightPerformed;
            controls.Menu.Select.performed += OnSelectPerformed;
            controls.Menu.Cancel.performed += OnCancelPerformed;


        }

        private void Start() {
            curMenuNav = MenuPanels[menuNavIndex].GetComponent<IMenuNavigation>();
            curMenuNav.LoadMenu();
        }




        #region Controller inputs 
        private void OnUpPerformed(InputAction.CallbackContext context) {
            if (!canNavigateMenu) return;
            curMenuNav.OnNavUp();
        }
        private void OnDownPerformed(InputAction.CallbackContext context) {
            if (!canNavigateMenu) return;
            curMenuNav.OnNavDown();
        }
        private void OnLeftPerformed(InputAction.CallbackContext context) {
            if (!canNavigateMenu) return;
            curMenuNav.OnNavLeft();
        }
        private void OnRightPerformed(InputAction.CallbackContext context) {
            if (!canNavigateMenu) return;
            curMenuNav.OnNavRight();
        }

        private void OnSelectPerformed(InputAction.CallbackContext context) {
            if (!canNavigateMenu) return;
            curMenuNav.OnNavSelect();
        }
        private void OnCancelPerformed(InputAction.CallbackContext context) {
            if (!canNavigateMenu) return;
            curMenuNav.OnNavCancel();
        }
        #endregion




        public IEnumerator GoToNextMenu() {

            canNavigateMenu = false;
            yield return StartCoroutine(Fade(blkOverlay, 1.0f)); // fade to blk
            curMenuNav.UnloadMenu();
            menuNavIndex++;
            curMenuNav = MenuPanels[menuNavIndex].GetComponent<IMenuNavigation>();
            curMenuNav.LoadMenu();
            yield return StartCoroutine(Fade(blkOverlay, 0.0f)); // fade to menu
            canNavigateMenu = true;

        }

        public IEnumerator GoToPrevMenu() {
            canNavigateMenu = false;
            yield return StartCoroutine(Fade(blkOverlay, 1.0f)); // fade to blk
            curMenuNav.UnloadMenu();
            menuNavIndex--;
            curMenuNav = MenuPanels[menuNavIndex].GetComponent<IMenuNavigation>();
            curMenuNav.LoadMenu();
            yield return StartCoroutine(Fade(blkOverlay, 0.0f)); // fade to menu
            canNavigateMenu = true;

        }


        private IEnumerator Fade(CanvasGroup canvasGroup, float targetAlpha)
        {
            canvasGroup.blocksRaycasts = true;

            while (!Mathf.Approximately(canvasGroup.alpha, targetAlpha))
            {
                canvasGroup.alpha = Mathf.MoveTowards(
                    canvasGroup.alpha,
                    targetAlpha,
                    (1f / fadeDuration) * Time.unscaledDeltaTime
                );

                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            canvasGroup.blocksRaycasts = targetAlpha > 0f;
        }






        public void PlayAudioOneShot(MenuAudioType a) {
            switch (a) {
                case MenuAudioType.select:
                    audioSource.volume = 0.5f;
                    audioSource.PlayOneShot(selectSfx);
                    break;
                case MenuAudioType.bigSelect:
                    audioSource.volume = 0.5f;
                    audioSource.PlayOneShot(bigSelectSfx);
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

        public int GetStartingClassesSize() { 
            return startingClasses.Length;
        }

        public StartingClassInfo GetClassInfo(int i) { 
            return startingClasses[i];
        }

        [System.Serializable]
        public struct StartingClassInfo {
            [SerializeField] public string className;
            [SerializeField] public Sprite classCharaImage_Sq;
            [TextArea(4, 20)]
            [SerializeField] public string classDesc;
            [SerializeField] public Sprite classCharaImage;
            [SerializeField] public int startingHp;
            [SerializeField] public int startingMp;
            [SerializeField] public int startingSp;
            [SerializeField] public string startingName;
            [SerializeField] public string classOptionTitle;
            [SerializeField] public SubClassOptionInfo[] subClassOptions;
        }

        [System.Serializable]
        public struct SubClassOptionInfo {
            [SerializeField] public string optionName;
            [SerializeField] public Sprite optionIcon;
            [TextArea(4, 20)]
            [SerializeField] public string optionDesc;
        }


        [System.Serializable]
        public struct HeirloomItemInfo
        {
            [SerializeField] public string itemName;
            [TextArea(4, 20)]
            [SerializeField] public string itemDesc;
            [SerializeField] public Sprite itemIcon;
            
        }


    }



}
