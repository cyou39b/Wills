using UnityEditor;
using UnityEngine;

public class MenuInMining : MonoBehaviour
{
    public GameObject MenuButton;
    public GameObject MenuPanel;
    public static bool IsMenuPanelOpening = false;
    float timeScaleBeforeMenuPanelOpening; 
    int pid;
    void Start(){}

    void Update(){
        if (MapScenesSwicher.isMapOpening || MenuManager.IsMenuOpen){
            MenuButton.SetActive(false);
        }
        else{
            MenuButton.SetActive(true);
        }
    }
    public void OpenPanel(){
        MenuPanel.SetActive(true);
        IsMenuPanelOpening = true;
        timeScaleBeforeMenuPanelOpening = Time.timeScale;
        Time.timeScale = 0.0f;
        pid = UIStack.Instance.NewPanel(() =>{
            IsMenuPanelOpening = false;
            Time.timeScale = timeScaleBeforeMenuPanelOpening;
            MenuPanel.SetActive(false);
        });
    }
    public void ClosePanel(){
        UIStack.Instance.RemovePanel(pid);
    }
}