using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay_Hearts : MonoBehaviour
{

    public static HeartHealthSystem heartHealthSystemStatic;

    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Sprite[] heartSprites = new Sprite[5];

    [SerializeField] private AnimationClip heartFullAnimationClip;

    private List<HeartImage> heartImageList;
    private HeartHealthSystem heartHealthSystem;
    private bool isHealing;

    private float heartsHealingAnimatedTimer = 0.0f;
    private float heartsHealingAnimatedTime = 0.05f;

    private void Awake(){
        heartImageList = new List<HeartImage>();
    }

    void Start() {

        HeartHealthSystem heartHealthSystem = new HeartHealthSystem(18);
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

        //DEBUG TESTING
        if (Input.GetKeyDown(KeyCode.K))
        {
            heartHealthSystem.Heal(1);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            heartHealthSystem.Heal(4);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            heartHealthSystem.Heal(16);
        }


        //hearts healing Animated timer
        heartsHealingAnimatedTimer -= Time.deltaTime;
        if (heartsHealingAnimatedTimer <= 0.0f) {
            heartsHealingAnimatedTimer = heartsHealingAnimatedTime;
            HealingAnimatedPeriodic();
        }


    }





    public void SetHeartHealthSystem(HeartHealthSystem heartHealthSystem) {
        this.heartHealthSystem = heartHealthSystem;
        heartHealthSystemStatic = heartHealthSystem;

        List<HeartHealthSystem.Heart> heartList = this.heartHealthSystem.GetHeartList();
        //Vector2 heartAnchoredPos = new Vector2 (0, 0);

        int row = 0;
        int col = 0;
        int colMax = 10;
        int rowColSize = 18;

        for (int i = 0; i < heartList.Count; i++) { 
            HeartHealthSystem.Heart heart = heartList[i];
            Vector2 heartAnchoredPos = new Vector2(col * rowColSize, -row * rowColSize);
            CreateHeartImage(heartAnchoredPos).SetHeartFragment(heart.GetFragmentAmount());
           
            col++;
            if (col >= colMax) {
                row++;
                col = 0;
            }

        }

        this.heartHealthSystem.OnDamage += HeartHealthSystem_OnDamage;
        this.heartHealthSystem.OnHeal += HeartHealthSystem_OnHeal;
        this.heartHealthSystem.OnDead += HeartHealthSystem_OnDead;

    }

    private void HeartHealthSystem_OnDamage(object sender, System.EventArgs e) {
        //Heart health system was damaged
       RefreshAllHearts();
    }

    private void HeartHealthSystem_OnHeal(object sender, System.EventArgs e)
    {
        //Heart health system was healed
        //RefreshAllHearts();
        isHealing = true;
    }

    private void HeartHealthSystem_OnDead(object sender, System.EventArgs e) {
        Debug.Log("Player Dead");
    }


    private void RefreshAllHearts() {
        List<HeartHealthSystem.Heart> heartList = heartHealthSystem.GetHeartList();
        for (int i = 0; i < heartImageList.Count; i++)
        {
            HeartImage heartImage = heartImageList[i];
            HeartHealthSystem.Heart heart = heartList[i];
            heartImage.SetHeartFragment(heart.GetFragmentAmount());
        }
    }

    private void HealingAnimatedPeriodic() {
        if (!isHealing) return;
        bool fullyHealed = true;
        List<HeartHealthSystem.Heart> heartList = heartHealthSystem.GetHeartList();
        for (int i = 0; i < heartList.Count; i++)
        {
            HeartImage heartImage = heartImageList[i];
            HeartHealthSystem.Heart heart = heartList[i];
            if (heartImage.GetFragmentAmount() != heart.GetFragmentAmount()) {
                //visual is diff from logic
                heartImage.AddHeartImageFragment();

                if (heartImage.GetFragmentAmount() == HeartHealthSystem.MAX_FRAGMENT_AMOUNT) { 
                    //this heart was fully healed
                    heartImage.PlayHeartFullAnimation();
                
                }

                fullyHealed = false;
                break;
            }
        }

        if (fullyHealed) isHealing = false;

    }


    private HeartImage CreateHeartImage(Vector2 anchoredPos) { 

        //Create game obj
        GameObject heartGameObj = new GameObject("Heart",typeof(Image),typeof(Animation));
        //Set as child of this transform
        heartGameObj.transform.parent = transform;
        heartGameObj.transform.localPosition = Vector3.zero;

        //Set Image Size
        heartGameObj.GetComponent<RectTransform>().sizeDelta = new Vector2(32, 32);
        heartGameObj.GetComponent<RectTransform>().anchoredPosition = anchoredPos;

        heartGameObj.GetComponent<Animation>().AddClip(heartFullAnimationClip, "HeartFull");

        //Set Heart Sprite
        Image heartImageUI = heartGameObj.GetComponent<Image>();
        heartImageUI.sprite = heartSprite;
        
        HeartImage heartImage = new HeartImage(this,heartImageUI, heartGameObj.GetComponent<Animation>());
        heartImageList.Add(heartImage);

        return heartImage;
    
    }

    //represents a single heart
    public class HeartImage
    {
        private int fragments;
        private Image heartImage;
        private HealthDisplay_Hearts healthDisplay;
        private Animation animation;

        public HeartImage(HealthDisplay_Hearts healthDisplay, Image heartImage, Animation animation) { 
            this.healthDisplay = healthDisplay;
            this.heartImage = heartImage;
            this.animation = animation;
        }

        public void SetHeartFragment(int fragments) {
            this.fragments = fragments;
            heartImage.sprite = healthDisplay.heartSprites[fragments];
        }


        public int GetFragmentAmount() { 
            return fragments;
        }

        public void AddHeartImageFragment() {
            SetHeartFragment(fragments + 1);
        }

        public void PlayHeartFullAnimation() { 
            animation.Play("HeartFull",PlayMode.StopAll);
        }

    }

}
