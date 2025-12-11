using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuLogic : MonoBehaviour
{
    public GameObject OptionsScreen;
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // FIXME: Pressing Esc when binding key will cause this to also close options menu
            if (OptionsScreen.activeSelf)
            {
                this.CloseOptions();
            }
            else
            {
                this.ExitGame();
            }
        }
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
