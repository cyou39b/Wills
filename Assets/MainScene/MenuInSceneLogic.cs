using UnityEngine;

public class MenuInSceneLogic : MonoBehaviour{
    public GameObject MenuPanel;
    public GameObject MenuButton;
    public static bool IsSceneMenuOpen = false;
    float timeScaleBeforeMenuOpen;
    int MenuPid;
    void Update(){
        UpdateMenuButton();
    }
    public void OpenMenu(){
        if(MenuManager.IsMenuOpen){return;}

        MenuPid = UIStack.Instance.NewPanel(() =>{
            MenuPanel.SetActive(false);
            IsSceneMenuOpen = false;
            Time.timeScale = timeScaleBeforeMenuOpen;
        });

        MenuPanel.SetActive(true);
        IsSceneMenuOpen = true;
        timeScaleBeforeMenuOpen = Time.timeScale;
        Time.timeScale = 0.0f;
    }
    public void CloseMenu(){
        UIStack.Instance.RemovePanel(MenuPid);
    }
    void UpdateMenuButton(){
        if (MenuManager.IsMenuOpen){
            MenuButton.SetActive(false);
        }
        else{
            MenuButton.SetActive(true);
        }
    }
}