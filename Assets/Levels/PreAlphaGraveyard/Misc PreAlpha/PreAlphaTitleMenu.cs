using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class PreAlphaTitleMenu : MonoBehaviour
{
    [SerializeField] private Image[] mainMenuItems;
    [SerializeField] private int mainMenuIndex;

    [SerializeField] private Sprite menuItemBg;
    [SerializeField] private Sprite menuItemBg_highlighted;
    [SerializeField] private int menuItemWt = 130;
    [SerializeField] private int menuItemWt_highlighted = 150;

    [Header("Sfx")]
    [SerializeField] private AudioClip menuBgm;
    [SerializeField] private AudioClip crowSfx;
    [SerializeField] private AudioClip menuNavSfx;
    [SerializeField] private AudioClip menuSelectSfx;
    private AudioSource audioSource;

    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject aboutMenuPanel;

    [Header("Fade In/Out")]
    [SerializeField] private CanvasGroup fadeOutCanvasGroup;
    [SerializeField] private CanvasGroup logoCanvasGroup;
    [SerializeField] private float fadeDuration = 0.35f;

    private PlayerInputActions controls;

    private TitleMenuState state = TitleMenuState.Intro;

    public enum TitleMenuState { 
        Intro,
        MainMenu,
        StartNewGame,
        LoadGameMenu,
        OptionsMenu,
        AboutMenu
    
    }


    #region On Enable/Disable
    private void OnEnable(){
        controls.Enable();
    }

    private void OnDisable(){
        controls.Disable();
    }
    #endregion

    void Awake(){

        mainMenuIndex = 0;  
        UpdateMainMenu();

        Cursor.visible = false;

        controls = new PlayerInputActions();

        controls.Menu.Up.performed += OnUpPerformed;
        controls.Menu.Down.performed += OnDownPerformed;
        controls.Menu.Select.performed += OnSelectPerformed;

        audioSource = GetComponent<AudioSource>();

    }

    private void Start(){
        StartCoroutine(MenuIntro());
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit(); //debug
    }

    private void OnUpPerformed(InputAction.CallbackContext context){

        if (state != TitleMenuState.MainMenu) return;
        audioSource.PlayOneShot(menuNavSfx);
        
        mainMenuIndex--;
        if (mainMenuIndex < 0) {
            mainMenuIndex = 0;
        }
        UpdateMainMenu();
    }
    private void OnDownPerformed(InputAction.CallbackContext context)
    {
        if (state != TitleMenuState.MainMenu) return;
        audioSource.PlayOneShot(menuNavSfx);
       
        mainMenuIndex++;
        if (mainMenuIndex > mainMenuItems.Length-1) { 
            mainMenuIndex = mainMenuItems.Length-1;
        }
        UpdateMainMenu();
    }
    private void OnSelectPerformed(InputAction.CallbackContext context){
        Debug.Log("menu select pressed");
        audioSource.PlayOneShot(menuSelectSfx);
        if (state == TitleMenuState.MainMenu){

            switch (mainMenuIndex){
                case 0:
                    Debug.Log("start new game");
                    StartCoroutine(StartNewGame());
                    break;
                case 1:
                    Debug.Log("load game");
                    break;
                case 2:
                    Debug.Log("Options");
                    break;
                case 3:
                    Debug.Log("about Menu");
                    aboutMenuPanel.SetActive(true);
                    state = TitleMenuState.AboutMenu;
                    break;
                case 4:
                    Debug.Log("quit game");
                    Application.Quit();
                    break;

            }
        }else if (state == TitleMenuState.AboutMenu) { 
            aboutMenuPanel.SetActive(false);
            state = TitleMenuState.MainMenu;
        }

    }

    private void UpdateMainMenu() {

        foreach (Image item in mainMenuItems) {
            item.sprite = menuItemBg;
            item.rectTransform.sizeDelta = new Vector2(menuItemWt, item.rectTransform.sizeDelta.y);
        }

        mainMenuItems[mainMenuIndex].sprite = menuItemBg_highlighted;
        mainMenuItems[mainMenuIndex].rectTransform.sizeDelta = new Vector2(menuItemWt_highlighted, mainMenuItems[mainMenuIndex].rectTransform.sizeDelta.y);
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

    private IEnumerator MenuIntro() { 
        
        yield return new WaitForSeconds(2.0f);
        audioSource.PlayOneShot(crowSfx);
        yield return StartCoroutine(Fade(logoCanvasGroup, 1.0f)); // fade in logo

        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(Fade(logoCanvasGroup, 0.0f)); // fade out logo

        audioSource.clip = menuBgm;
        audioSource.Play();
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(Fade(fadeOutCanvasGroup,0.0f)); // fade in
        yield return new WaitForSeconds(1.0f);

        Debug.Log("Intro Over");
        state = TitleMenuState.MainMenu;
        mainMenuPanel.SetActive(true);
    
    }

    private IEnumerator StartNewGame(){
        state = TitleMenuState.StartNewGame;
        yield return StartCoroutine(Fade(fadeOutCanvasGroup, 1.0f)); // fade out
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(1);

    }


}
