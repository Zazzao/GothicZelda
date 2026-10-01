
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

        private void Start()
        {
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


        private IEnumerator WaitForIconRotation() { 
            yield return new WaitForSeconds(0.6f);

            charClassImages[0].color = Color.white;
            selectedClassName.text = menuController.GetClassInfo(charClassIndex).className; 
            menuController.playerName = menuController.GetClassInfo(charClassIndex).startingName;
            menuController.playerCharaImage = menuController.GetClassInfo(charClassIndex).classCharaImage_Sq;
            bannerText.text = menuController.GetClassInfo(charClassIndex).className;
            bannerPanel.SetActive(true);


            yield return new WaitForSeconds(0.2f);
            canNavMenu = true;
            
        }



        #region I menu Nav

        public void OnNavDown()
        {
            //throw new System.NotImplementedException();
        }

        public void OnNavLeft()
        {
            if (!canNavMenu) return;
            canNavMenu = false;
            menuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.move);
            charClassIndex--;
            if (charClassIndex < 0) charClassIndex = charClassImages.Count-1;

            bannerPanel.SetActive(false);

            Image lastItem = charClassImages[0];
            charClassImages.RemoveAt(0);
            charClassImages.Add(lastItem);
            foreach (var image in charClassImages) { 
                image.color = inactiveIconColor;
            }

            for (int i = 0; i < charClassImages.Count; i++)
            {
                charClassImages[i].transform.SetParent(classIconSlots[i].transform, true);
                StartCoroutine(RotateCharClassIcons(
                    charClassImages[i].rectTransform,
                    charClassImages[i].rectTransform.localPosition,
                    Vector2.zero));

            }
            StartCoroutine(WaitForIconRotation());
        }

        public void OnNavRight()
        {
            if (!canNavMenu) return;
            canNavMenu = false;
            menuController.PlayAudioOneShot(Test_CharacterSelectMenuController.MenuAudioType.move);
            charClassIndex++;
            if (charClassIndex >= charClassImages.Count) charClassIndex = 0;

            bannerPanel.SetActive(false);

            Image lastItem = charClassImages[charClassImages.Count - 1];
            charClassImages.RemoveAt(charClassImages.Count - 1);
            charClassImages.Insert(0, lastItem);

            foreach (var image in charClassImages)
            {
                image.color = inactiveIconColor;
            }

            for (int i = 0; i < charClassImages.Count; i++)
            {
                charClassImages[i].transform.SetParent(classIconSlots[i].transform, true);
                StartCoroutine(RotateCharClassIcons(
                    charClassImages[i].rectTransform,
                    charClassImages[i].rectTransform.localPosition,
                    Vector2.zero));

            }
            StartCoroutine(WaitForIconRotation());
        }

        

        public void OnNavUp()
        {
            //throw new System.NotImplementedException();
        }


        public void OnNavSelect()
        {
            if (!canNavMenu) return;
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
            // throw new System.NotImplementedException();
        }

        public void UnloadMenu()
        {
            rootPanel.SetActive(false);
            //throw new System.NotImplementedException();
        }
        #endregion


    }
}
