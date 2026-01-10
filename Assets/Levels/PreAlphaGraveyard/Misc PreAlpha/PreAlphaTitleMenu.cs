using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PreAlphaTitleMenu : MonoBehaviour
{
    [SerializeField] private Image[] mainMenuItems;
    [SerializeField] private int mainMenuIndex;

    [SerializeField] private Sprite menuItemBg;
    [SerializeField] private Sprite menuItemBg_highlighted;
    [SerializeField] private int menuItemWt = 130;
    [SerializeField] private int menuItemWt_highlighted = 150;

    [Header("Sfx")]
    [SerializeField] private AudioClip menuNavSfx;
    private AudioSource audioSource;

    [Header("Menu Panels")]
    [SerializeField] private GameObject aboutMenuPanel;


    private PlayerInputActions controls;

    private TitleMenuState state = TitleMenuState.MainMenu;

    public enum TitleMenuState { 
        Intro,
        MainMenu,
        LoadGameMenu,
        OptionsMenu,
        AboutMenu
    
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

    void Awake()
    {

        //Screen.SetResolution(640, 360,true);

        mainMenuIndex = 0;  
        UpdateMainMenu();

        controls = new PlayerInputActions();

        controls.Menu.Up.performed += OnUpPerformed;
        controls.Menu.Down.performed += OnDownPerformed;
        controls.Menu.Select.performed += OnSelectPerformed;

        audioSource = GetComponent<AudioSource>();

    }

    private void Update()
    {
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
    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("menu select pressed");

        if (state == TitleMenuState.MainMenu)
        {

            switch (mainMenuIndex)
            {
                case 0:
                    Debug.Log("start new game");
                    SceneManager.LoadScene(1);
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    aboutMenuPanel.SetActive(true);
                    state = TitleMenuState.AboutMenu;
                    break;
                case 4:
                    Debug.Log("quit game");
                    Application.Quit();
                    break;



            }
        }
        else if (state == TitleMenuState.AboutMenu) { 
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

}
