 using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuLogic : MonoBehaviour
{
    public GameObject OptionsScreen;
    void Start(){}
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            this.ExitGame();
        }
    }
    public void ExitGame()
    {
        Debug.Log("goodbye");
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
