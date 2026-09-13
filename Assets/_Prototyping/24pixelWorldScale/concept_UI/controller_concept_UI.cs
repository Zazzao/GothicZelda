using UnityEngine;



/// <summary>
/// this script handles controlling the concept scene for the 24 pixel UI test. 
/// It is NOT ment to be in the final build. this just makes cycling thought examples 
/// of UI concepts easier
/// </summary>



namespace DensetsuEngine.Prototyping
{
    public class controller_concept_UI : MonoBehaviour {

        [SerializeField] private bool showDebug = false;
        [SerializeField] private GameObject debugPanel;



        public enum SceneState { 
            none,
            inventory,
            dialogue,

        }



        void Start(){

        }


        void Update()
        {

            if (showDebug && debugPanel.activeInHierarchy == false) { 
                debugPanel.SetActive(true);
            }
            if (!showDebug && debugPanel.activeInHierarchy == true)
            {
                debugPanel.SetActive(false);
            }


        }







    }
}
