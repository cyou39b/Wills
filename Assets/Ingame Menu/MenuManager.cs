using UnityEngine;
using UnityEngine.SceneManagement;

// 在遊戲進行中的menu

public class MenuManager : MonoBehaviour
{
    public GameObject Blur;
    public GameObject MenuScreen;
    public GameObject PauseButton;
    public GameObject CloseMenuButton;

    private float timeScaleBefaorePause;
    public static bool IsMenuOpen = false;

    public void Awake()
    {
        // By making these GameObjects actived and then deactive them on awake
        // can bring the initialize work forward to load scene, and reduce lag
        // when opening menu the first time.
        Blur.SetActive(false);
        MenuScreen.SetActive(false);
        CloseMenuButton.SetActive(false);

        UIStack.Instance.emptyAction += OpenMenu;
    }

    private int pid;
    public void OpenMenu()
    {
        if( OptionMenu.IsBinding || 
            MapScenesSwicher.isMapOpening || 
            DeathManager.Activated ||
            DialogueManager.IsTalking||
            ShopInterfaceLogic.isBuying ||
            BackpackLogic.IsBackpackOpening ||
            MenuInSceneLogic.IsSceneMenuOpen ||
            MenuInMining.IsMenuPanelOpening
        ) {return;}

        pid = UIStack.Instance.NewPanel(
            () =>
            {
                if( OptionMenu.IsBinding || 
                    MapScenesSwicher.isMapOpening || 
                    DeathManager.Activated)
                    {return;}
                IsMenuOpen = false;
                Time.timeScale = timeScaleBefaorePause ;
                Blur.SetActive(false);
                MenuScreen.SetActive(false);
                PauseButton.SetActive(true);
                CloseMenuButton.SetActive(false);
            }
        );

        IsMenuOpen = true;

        timeScaleBefaorePause = Time.timeScale;
        Time.timeScale = 0.0f;

        Blur.SetActive(true);
        MenuScreen.SetActive(true);
        PauseButton.SetActive(false);
        CloseMenuButton.SetActive(true);
    }
    
    public void CloseMenu()
    {
        UIStack.Instance.RemovePanel(pid);
    }

    public void ExitScene()
    {
        LoadSceneManager.NextScene = "MainMenu";
        SceneManager.LoadScene("LoadSceneBuffer");
    }
}
