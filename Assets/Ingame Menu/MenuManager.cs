using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class MenuManager : MonoBehaviour
{
    public GameObject Blur;
    public GameObject MenuScreen;
    public GameObject PauseButton;
    public GameObject CloseMenuButton;
    private bool isMenuOpen = false;
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!OptionMenu.IsBinding)
            {
                if (isMenuOpen || MapScenesSwicher.isMapOpening)
                {
                    CloseMenu();
                }
                else
                {
                    OpenMenu();
                }
            }
        }
    }

    public void OpenMenu()
    {
        isMenuOpen = true;
        Time.timeScale = 0.0f;
        Blur.SetActive(true);
        MenuScreen.SetActive(true);
        PauseButton.SetActive(false);
        CloseMenuButton.SetActive(true);
    }
    
    public void CloseMenu()
    {
        isMenuOpen = false;
        Time.timeScale = 1.0f;
        Blur.SetActive(false);
        MenuScreen.SetActive(false);
        PauseButton.SetActive(true);
        CloseMenuButton.SetActive(false);
    }

    public void ExitScene()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            #warning TODO: Figure out what to do here.
        #endif
    }
}
