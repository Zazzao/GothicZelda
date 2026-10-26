
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace DensetsuEngine.Prototyping
{
    public class Test_CharGen_SelectClass : MonoBehaviour, IMenuNavigation
    {
        
        bool canNavMenu = true;
        bool inSubclassMenu = false;
        
        [SerializeField] private GameObject rootPanel;
        Test_CharacterSelectMenuController menuController;


        public Image testImage1;
        public Image testImage2;
        public Image testImage3;

        [SerializeField] List<Image> charClassImages = new List<Image>();
        [SerializeField] RectTransform[] classIconSlots;
        private int charClassIndex = 0;

        [SerializeField] Color activeIconColor;
        [SerializeField] Color inactiveIconColor;

        [Header("class Info Panel")]
        [SerializeField] TMP_Text selectedClassName;
        [SerializeField] TMP_Text selecetedClassDesc;
        [SerializeField] GameObject bannerPanel;
        [SerializeField] TMP_Text bannerText;

        [Header("Subclass Menu Panel")]
        [SerializeField] RectTransform classOptionCursor;
        [SerializeField] GameObject subClassOptionPanel;
        [SerializeField] GameObject subClassOptionIconPrfab;
        [SerializeField] GameObject subClassOptionIconGridPanel;
        [SerializeField] TMP_Text sublassMenuTitleText;
        [SerializeField] TMP_Text subclassOptionNameText;
        [SerializeField] TMP_Text subClassOptionDescriptionText;
        private List<GameObject> iconList = new List<GameObject>();
        private int iconIndex = 0;

        private void Start(){
            menuController = FindAnyObjectByType<Test_CharacterSelectMenuController>();
        }



        private IEnumerator RotateCharClassIcons(RectTransform rectTransform, Vector2 start, Vector2 end) {

            float duration = 0.4f;
            
            float elapsed = 0f;
            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Interpolate the anchored position
                rectTransform.localPosition = Vector2.Lerp(start, end, t);
                yield return null;

            }
            rectTransform.anchoredPosition = end;
        }

        private IEnumerator WaitForCarouselRotation() { 
            yield return new WaitForSeconds(0.6f);

            //TO-DO: These line repeatin in the load menu function
            charClassImages[0].color = Color.white;
            selectedClassName.text = menuController.GetClassInfo(charClassIndex).className; 
            menuController.playerName = menuController.GetClassInfo(charClassIndex).startingName;
            menuController.playerClass = charClassIndex;
            
            bannerText.text = menuController.GetClassInfo(charClassIndex).className;
            bannerPanel.SetActive(true);


            yield return new WaitForSeconds(0.2f);
            canNavMenu = true;
            
        }






        #region I menu Nav

        #region not used
        public void OnNavUp(){} // do nothing

        public void OnNavDown(){} // do nothing
        #endregion


        public void OnNavLeft(){
            if (!canNavMenu) return;

            if (inSubclassMenu){
                OnClassOptionLeft();
            }else {
                OnClassCarouselLeft();
            }
                
           
        }

        public void OnNavRight(){
            if (!canNavMenu) return;

            if (inSubclassMenu){
                OnClassOptionRight();
            }
            else{
                OnClassCarouselRight();
            }
        }

        


        public void OnNavSelect()
        {
            if (!canNavMenu) return;

            if (!inSubclassMenu)
            {  
               StartCoroutine(OpenClassOptionMenu());
            }
            else {
                menuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.bigSelect);
                StartCoroutine(menuController.GoToNextMenu());
            }


        }

        public void OnNavCancel()
        {
            if (!canNavMenu) return;
            if (!inSubclassMenu)
            {
                StartCoroutine(menuController.GoToPrevMenu());
            }
            else {
                CloseClassOptionMenu();
            }

            
        }

        public void LoadMenu()
        {
            CloseClassOptionMenu();

            //set color of carosel
            foreach (var image in charClassImages){
                image.color = inactiveIconColor;
            }
            charClassImages[0].color = Color.white;
            selectedClassName.text = menuController.GetClassInfo(charClassIndex).className;
            bannerText.text = menuController.GetClassInfo(charClassIndex).className;

            menuController.playerName = menuController.GetClassInfo(charClassIndex).startingName;
            menuController.playerClass = charClassIndex;


            rootPanel.SetActive(true);
            // throw new System.NotImplementedException();
        }

        public void UnloadMenu()
        {
            rootPanel.SetActive(false);
            //throw new System.NotImplementedException();
        }
        #endregion


        private void OnClassCarouselLeft() {
            charClassIndex--;
            if (charClassIndex < 0) charClassIndex = charClassImages.Count - 1;

            Image lastItem = charClassImages[0];
            charClassImages.RemoveAt(0);
            charClassImages.Add(lastItem);


            UpdateClassCarousel();
        }

        private void OnClassCarouselRight() {
            charClassIndex++;
            if (charClassIndex >= charClassImages.Count) charClassIndex = 0;

            Image lastItem = charClassImages[charClassImages.Count - 1];
            charClassImages.RemoveAt(charClassImages.Count - 1);
            charClassImages.Insert(0, lastItem);

            UpdateClassCarousel();
        }

        private void UpdateClassCarousel() {

            canNavMenu = false;
            menuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.move);
            bannerPanel.SetActive(false);

            foreach (var image in charClassImages){
                image.color = inactiveIconColor;
            }

            for (int i = 0; i < charClassImages.Count; i++){
                charClassImages[i].transform.SetParent(classIconSlots[i].transform, true);
                StartCoroutine(RotateCharClassIcons(
                    charClassImages[i].rectTransform,
                    charClassImages[i].rectTransform.localPosition,
                    Vector2.zero));

            }
            StartCoroutine(WaitForCarouselRotation());

        }



        #region Class Option Menu Functions
        private IEnumerator OpenClassOptionMenu(){
           
            inSubclassMenu = true;
            sublassMenuTitleText.text = menuController.GetClassInfo(charClassIndex).classOptionTitle;

            //create subclass option icons list
            for (int i = 0; i < menuController.GetClassInfo(charClassIndex).subClassOptions.Length; i++){
                GameObject newIcon = Instantiate(subClassOptionIconPrfab, subClassOptionIconGridPanel.transform);
               
                //TO-DO: add icon image out option
                iconList.Add(newIcon);
            }


            //init frist option
            iconIndex = 0;
            var classOptionInfo = menuController.GetClassInfo(charClassIndex).subClassOptions[0];
            subclassOptionNameText.text = classOptionInfo.optionName;
            subClassOptionDescriptionText.text = classOptionInfo.optionDesc;

            subClassOptionPanel.SetActive(true);
            yield return null;

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                subClassOptionIconGridPanel.GetComponent<RectTransform>()
            );

            // Set cursor position
            RectTransform firstIcon = iconList[0].GetComponent<RectTransform>();
            classOptionCursor.anchoredPosition = firstIcon.localPosition;

           // Debug.Log("First icon position: " + firstIcon.localPosition);

        }



        private void CloseClassOptionMenu()
        {
            inSubclassMenu = false;
            subClassOptionPanel.SetActive(false);
            foreach (GameObject icon in iconList){
                Destroy(icon);
            }
            iconList.Clear();
        }


        private void OnClassOptionLeft() {
            iconIndex--;
            if (iconIndex < 0) iconIndex = 0;
            subclassOptionNameText.text = menuController.GetClassInfo(charClassIndex).subClassOptions[iconIndex].optionName;
            subClassOptionDescriptionText.text = menuController.GetClassInfo(charClassIndex).subClassOptions[iconIndex].optionDesc;

            //classOptionCursor.anchoredPosition = iconList[iconIndex].transform.localPosition;
            classOptionCursor.anchoredPosition = iconList[iconIndex].GetComponent<RectTransform>().localPosition;

        }
        private void OnClassOptionRight() {
            iconIndex++;
            if (iconIndex >= iconList.Count) iconIndex = iconList.Count-1;

            subclassOptionNameText.text = menuController.GetClassInfo(charClassIndex).subClassOptions[iconIndex].optionName;
            subClassOptionDescriptionText.text = menuController.GetClassInfo(charClassIndex).subClassOptions[iconIndex].optionDesc;
          
            classOptionCursor.anchoredPosition = iconList[iconIndex].GetComponent<RectTransform>().localPosition;
            
        }

        #endregion
    }
}
