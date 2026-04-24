using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 在main menu的Logic，提供canvas上的button的callback function

public class MainMenuLogic : MonoBehaviour
{
    public GameObject OptionsScreen;
    public Text StartGameButtonText;
    public DataSave dataSave;

    void Start()
    {
        StartGameButtonText.text = (DataSave.DataBuffer != null)
            ? "Load Game"
            : "Start New Game"; 
    }

    public void StartGame()
    {
        if(DataSave.DataBuffer == null)
        {
            // TODO: Sort of figure out what's the right thing to do to start a new game.
            LoadSceneManager.NextScene = "MainScene";
            SceneManager.LoadScene("LoadSceneBuffer");
        }
        else
        {
            LoadSceneManager.NextScene = DataSave.DataBuffer.CurrentSceneName;
            SceneManager.LoadScene("LoadSceneBuffer");
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
