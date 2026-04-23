using UnityEditor;
using UnityEngine;

public class MenuInMining : MonoBehaviour
{
    public GameObject MenuButton;
    public GameObject MenuPanel;
    public static bool IsMenuPanelOpening = false;
    float timeScaleBeforeMenuPanelOpening; 
    void Start(){}

    void Update(){
        if (MapScenesSwicher.isMapOpening){
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
    }
    public void ClosePanel(){
        IsMenuPanelOpening = false;
        Time.timeScale = timeScaleBeforeMenuPanelOpening;
        MenuPanel.SetActive(false);
    }
}