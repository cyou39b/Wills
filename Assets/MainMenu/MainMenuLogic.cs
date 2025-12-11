using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuLogic : MonoBehaviour
{
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
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
