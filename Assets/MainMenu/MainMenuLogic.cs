using UnityEditor;
using UnityEngine;
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

        UIStack.emptyAction = OpenOptions;
    }

    public void StartGame()
    {
        if(DataSave.DataBuffer == null)
        {
            // TODO: Sort of figure out what's the right thing to do to start a new game.
            GlobalVariables.Instance.mainScenePosition = new Vector3(-17.0f, -18.0f);
            LoadSceneManager.LoadBufferAndLoadScene("MainScene");
        }
        else
        {
            LoadSceneManager.LoadBufferAndLoadScene(DataSave.DataBuffer.CurrentSceneName);
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

    private int pid;
    public void OpenOptions(){
        OptionsScreen.SetActive(true);
        pid = UIStack.Instance.NewPanel(
            () =>
            {
                OptionsScreen.SetActive(false);
            }
        );
    }
    public void CloseOptions()
    {
        UIStack.Instance.RemovePanel(pid);
    }
}
