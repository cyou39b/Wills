using UnityEngine;

public class MenuInSceneLogic : MonoBehaviour{
    public GameObject MenuPanel;
    public static bool IsSceneMenuOpen = false;
    float timeScaleBeforeMenuOpen;
    void Update(){}
    public void OpenMenu(){
        MenuPanel.SetActive(true);
        IsSceneMenuOpen = true;
        timeScaleBeforeMenuOpen = Time.timeScale;
        Time.timeScale = 0.0f;
    }
    public void CloseMenu(){
        MenuPanel.SetActive(false);
        IsSceneMenuOpen = false;
        Time.timeScale = timeScaleBeforeMenuOpen;
    }
}