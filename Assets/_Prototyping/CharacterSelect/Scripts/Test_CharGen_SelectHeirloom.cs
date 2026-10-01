using UnityEngine;

namespace DensetsuEngine.Prototyping
{
    public class Test_CharGen_SelectHeirloom : MonoBehaviour, IMenuNavigation
    {

        Test_CharacterSelectMenuController menuController;
        [SerializeField] private GameObject rootPanel;

        private void Start()
        {
            menuController = FindAnyObjectByType<Test_CharacterSelectMenuController>();
        }



        public void OnNavDown()
        {
            //throw new System.NotImplementedException();
        }

        public void OnNavLeft()
        {
            //throw new System.NotImplementedException();
        }

        public void OnNavRight()
        {
            //throw new System.NotImplementedException();
        }

        public void OnNavUp()
        {
            //throw new System.NotImplementedException();
        }

        public void OnNavSelect()
        {
            menuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.bigSelect);
            StartCoroutine(menuController.GoToNextMenu());
        }

        public void OnNavCancel()
        {
            
            StartCoroutine(menuController.GoToPrevMenu());
        }

        public void LoadMenu()
        {
            rootPanel.SetActive(true);
            //throw new System.NotImplementedException();
        }

        public void UnloadMenu()
        {
            rootPanel.SetActive(false);
            //throw new System.NotImplementedException();
        }
    }
}
