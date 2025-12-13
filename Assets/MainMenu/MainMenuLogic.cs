using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuLogic : MonoBehaviour
{
    public GameObject OptionsScreen;
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
