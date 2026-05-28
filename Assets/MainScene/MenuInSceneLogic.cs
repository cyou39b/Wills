using UnityEngine;

public class MenuInSceneLogic : MonoBehaviour{
    public GameObject MenuPanel;
    public GameObject MenuButton;
    public static bool IsSceneMenuOpen = false;
    float timeScaleBeforeMenuOpen;
    int MenuPid;
    public GameObject InteractButton1;
    public GameObject InteractButton2;
    public static bool HaveToCloseInteractButton = false;
    void Update(){
        UpdateMenuButton();
        UpdateInteractButton();
    }
    public void OpenMenu(){
        if(MenuManager.IsMenuOpen){return;}

        MenuPid = UIStack.Instance.NewPanel(() =>{
            MenuPanel.SetActive(false);
            IsSceneMenuOpen = false;
            Time.timeScale = timeScaleBeforeMenuOpen;
            HaveToCloseInteractButton = false;
        });

        MenuPanel.SetActive(true);
        IsSceneMenuOpen = true;
        timeScaleBeforeMenuOpen = Time.timeScale;
        Time.timeScale = 0.0f;
        HaveToCloseInteractButton = true;
    }
    public void CloseMenu(){
        UIStack.Instance.RemovePanel(MenuPid);
    }
    void UpdateMenuButton(){
        if (MenuManager.IsMenuOpen || DialogueManager.IsTalking){
            MenuButton.SetActive(false);
        }
        else{
            MenuButton.SetActive(true);
        }
    }
    void UpdateInteractButton(){
        if (HaveToCloseInteractButton){
            InteractButton1.SetActive(false);
            InteractButton2.SetActive(false);
        }
        else if (!HaveToCloseInteractButton && JackMainScene.isInInteractRange2){
            InteractButton1.SetActive(true);
            InteractButton2.SetActive(true);
        }
        else if (!HaveToCloseInteractButton && JackMainScene.isInInteractRange1){
            InteractButton1.SetActive(true);
        }
    }
    
}