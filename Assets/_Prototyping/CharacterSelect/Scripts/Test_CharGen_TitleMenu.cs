
using UnityEngine;
using UnityEngine.UI;


namespace DensetsuEngine.Prototyping
{
    public class Test_CharGen_TitleMenu : MonoBehaviour, IMenuNavigation
    {

        Test_CharacterSelectMenuController menuController;
        [SerializeField] private GameObject rootPanel;
        [SerializeField] private GameObject fogPanel;

        [SerializeField] private GameObject[] menuButtons;
        [SerializeField] private int menuButtonIndex;

        [SerializeField] private GameObject aboutMenu;
        bool showAboutMenu = false;


        


        //OBS
        private SubMenuState _subMenuState = SubMenuState.startbutton;

        private enum SubMenuState { 
            none,
            startbutton,
            aboutButton,
            quitButton,

        }

        private void Start()
        {
            menuController = FindAnyObjectByType<Test_CharacterSelectMenuController>();
        }



        #region I MenuNavigation

        public void OnNavUp(){
            if (showAboutMenu) return;
            menuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.move);
            menuButtons[menuButtonIndex].GetComponent<Image>().color = Color.white;
            menuButtonIndex--;
            if (menuButtonIndex < 0) menuButtonIndex = 0;
            menuButtons[menuButtonIndex].GetComponent<Image>().color = Color.red;

        }

        public void OnNavDown(){
            if (showAboutMenu) return;
            menuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.move);
            menuButtons[menuButtonIndex].GetComponent<Image>().color = Color.white;
            menuButtonIndex++;
            if (menuButtonIndex >= menuButtons.Length) menuButtonIndex = menuButtons.Length-1;
            menuButtons[menuButtonIndex].GetComponent<Image>().color = Color.red;
        }

        public void OnNavLeft()
        {
            //throw new System.NotImplementedException();
        }

        public void OnNavRight()
        {
            //throw new System.NotImplementedException();
        }

      
        public void OnNavSelect(){
            switch (menuButtonIndex) { 
                case 0: // Start button
                    menuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.bigSelect);
                    StartCoroutine(menuController.GoToNextMenu());
                    break;
                case 1: //about button
                    menuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.select);
                    showAboutMenu = !showAboutMenu;
                    aboutMenu.SetActive(showAboutMenu);

                    break;
                case 2: // quit button
                    Application.Quit();
                    break;
            }
            
        }

        public void OnNavCancel(){
           // do nothing
        }

        public void LoadMenu(){
            
            menuButtonIndex = 0;
            menuButtons[menuButtonIndex].GetComponent<Image>().color = Color.red;

            rootPanel.SetActive(true);
            fogPanel.SetActive(false);
        }

        public void UnloadMenu(){
            rootPanel.SetActive(false);
            fogPanel.SetActive(true);
        }

        #endregion


    }
}
