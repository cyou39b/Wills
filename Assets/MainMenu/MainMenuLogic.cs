using UnityEditor;
using UnityEngine;

// 在main menu的Logic，提供canvas上的button的callback function

public class MainMenuLogic : MonoBehaviour
{
    public GameObject OptionsScreen;
    public void StartGame()
    {
        Debug.Log("StartGame pressed.");
    }
    public void ExitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    public void OpenOptions(){
        OptionsScreen.SetActive(true);
    }
    public void CloseOptions(){
        OptionsScreen.SetActive(false);
    }
}
