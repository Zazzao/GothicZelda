using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay_Hearts : MonoBehaviour
{

    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Sprite[] heartSprites = new Sprite[5];

    private HeartHealthSystem heartHealthSystem;
    private List<HeartImage> heartImageList;


    private void Awake(){
        heartImageList = new List<HeartImage>();
    }

    void Start() {

        HeartHealthSystem heartHealthSystem = new HeartHealthSystem(8);
        SetHeartHealthSystem(heartHealthSystem);
        
        
    }

    private void Update()
    {
        //DEBUG TESTING
        if (Input.GetKeyDown(KeyCode.A)) {
            heartHealthSystem.Damage(1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            heartHealthSystem.Damage(4);
        }
    }





    public void SetHeartHealthSystem(HeartHealthSystem heartHealthSystem) {
        this.heartHealthSystem = heartHealthSystem;

        List<HeartHealthSystem.Heart> heartList = this.heartHealthSystem.GetHeartList();
        Vector2 heartAnchoredPos = new Vector2 (0, 0);
        for (int i = 0; i < heartList.Count; i++) { 
            HeartHealthSystem.Heart heart = heartList[i];
            CreateHeartImage(heartAnchoredPos).SetHeartFragment(heart.GetFragmentAmount());
            heartAnchoredPos += new Vector2(20, 0);
        }

        this.heartHealthSystem.OnDamage += HeartHealthSystem_OnDamage;

    }

    private void HeartHealthSystem_OnDamage(object sender, System.EventArgs e) {
        //Heart health system was damaged
        List<HeartHealthSystem.Heart> heartList = heartHealthSystem.GetHeartList();
        for (int i = 0;i<heartImageList.Count;i++){
            HeartImage heartImage = heartImageList[i];
            HeartHealthSystem.Heart heart = heartList[i];
            heartImage.SetHeartFragment(heart.GetFragmentAmount());
        }
    }


    private HeartImage CreateHeartImage(Vector2 anchoredPos) { 

        //Create game obj
        GameObject heartGameObj = new GameObject("Heart",typeof(Image));
        //Set as child of this transform
        heartGameObj.transform.parent = transform;
        heartGameObj.transform.localPosition = Vector3.zero;

        //Set Image Size
        heartGameObj.GetComponent<RectTransform>().sizeDelta = new Vector2(32, 32);
        heartGameObj.GetComponent<RectTransform>().anchoredPosition = anchoredPos;

        //Set Heart Sprite
        Image heartImageUI = heartGameObj.GetComponent<Image>();
        heartImageUI.sprite = heartSprite;
        
        HeartImage heartImage = new HeartImage(this,heartImageUI);
        heartImageList.Add(heartImage);

        return heartImage;
    
    }

    //represents a single heart
    public class HeartImage
    {
        private Image heartImage;
        private HealthDisplay_Hearts healthDisplay;

        public HeartImage(HealthDisplay_Hearts healthDisplay, Image heartImage) { 
            this.healthDisplay = healthDisplay;
            this.heartImage = heartImage;
        }

        public void SetHeartFragment(int fragments) {
            heartImage.sprite = healthDisplay.heartSprites[fragments];
        }
    }

}
