using UnityEngine;
using UnityEngine.UI;




namespace DensetsuEngine.Prototyping
{
    



    public class Test_CharGen_ConfirmChar : MonoBehaviour, IMenuNavigation
    {

        [SerializeField] private GameObject rootPanel;

        private Test_CharacterSelectMenuController MenuController;

        private void Awake()
        {
            MenuController = FindAnyObjectByType<Test_CharacterSelectMenuController>();
        }

       

        public void OnNavUp()
        {
            //throw new System.NotImplementedException();
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
           //
        }

        public void OnNavSelect()
        {
            //
        }

        public void OnNavCancel()
        {

            StartCoroutine(MenuController.GoToPrevMenu());
        }

        public void LoadMenu()
        {
           rootPanel.SetActive(true);
            // throw new System.NotImplementedException();
        }

        public void UnloadMenu()
        {
           rootPanel.SetActive(false);
            //throw new System.NotImplementedException();
        }
    }
}
