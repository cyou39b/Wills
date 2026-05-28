using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class PlayerData{
    // basic data in game 
    public string CurrentSceneName;

// NOTE: Due some to technical issue, some fields loaded can't be used immediately in LoadData,
// NOTE: they might come with a "wasRead" variable to make it them be used in the right time.
    [System.NonSerialized]
    public bool positionWasRead = false;
    public bool PlayerHavePos;
    public Vector3 PlayerPos = Vector3.zero;

    // The direction player is facing if player is in scene mainScene or worldMine
    public int dir_idx0, dir_idx1;

    [Serializable]
    public struct PossessionPair
    {
        public int index;
        public int num;
    }
    public List<PossessionPair> AllPossessions = new List<PossessionPair>();

    public int Mine;

    // ------------ Player settings ---------------
    public int FrameRate;
    public int EXP; 

    public string JumpKey;
    public string MoveLeftKey;
    public string MoveRightKey;
    public string UpKey;
    public string DownKey;
    public string InteractKey;
    public string AttackKey;
    public string FindMineKey;
}

public class DataSave : MonoBehaviour{
    // NOTE: Inspector field
    public GameObject ConfirmPanel;
    int ConfirmPid;
    public GameObject FailedPanel;
    int FailedPid;
    public Text FailedPanelMsgText;

    static string SaveDataPath = null;
    public static PlayerData DataBuffer {get; private set;} = null;
    void Awake(){
        if(SaveDataPath == null) 
        {
            SaveDataPath = Application.persistentDataPath + "/save.json";
            Debug.Log(SaveDataPath);
        }

        if(DataBuffer == null)
        {
            if(ExistSavedGame())
            {
                LoadData();
            }
        }
    }

    [ContextMenu("Save Data")]
    public void SaveData(){
        DataBuffer ??= new PlayerData();

        DataBuffer.CurrentSceneName = SceneManager.GetActiveScene().name;

        GameObject player = GameObject.FindWithTag("Player");
        if(player == null)
        {
            DataBuffer.PlayerHavePos = false;
        }
        else
        {
            DataBuffer.PlayerHavePos = true;
            DataBuffer.PlayerPos = player.transform.position;
        }

        if(DataBuffer.CurrentSceneName == "MainScene")
        {
            JackMainScene jackMainScene = null;
            if(player == null || !player.TryGetComponent<JackMainScene>(out jackMainScene))
            {
                Debug.LogError("player == null || missing component");
            }
            DataBuffer.dir_idx0 = jackMainScene.idx0;
            DataBuffer.dir_idx1 = jackMainScene.idx1;
        }
        else if(DataBuffer.CurrentSceneName == "WorldMining")
        {
            JackMining jackMining = null;
            if(player == null || !player.TryGetComponent<JackMining>(out jackMining))
            {
                Debug.LogError("player == null || missing component");
            }
            DataBuffer.dir_idx0 = jackMining.idx0;
            DataBuffer.dir_idx1 = jackMining.idx1;
        }

        GlobalVariables.Instance.SaveKeys(DataBuffer);

        DataBuffer.Mine = GlobalVariables.Instance.NumMines;
        DataBuffer.EXP = GlobalVariables.Instance.EXP;

        DataBuffer.FrameRate = GlobalVariables.Instance.FrameRate;

        DataBuffer.AllPossessions.Clear();
        foreach(PossessionItems item in GlobalVariables.Instance.AllPossession)
        {
            DataBuffer.AllPossessions.Add(new PlayerData.PossessionPair()
            {
                index = item.index,
                num = item.Num
            });
        }

        string content = JsonUtility.ToJson(DataBuffer);
        try{
            FileInfo file = new FileInfo(SaveDataPath);
            file.Directory?.Create(); // Creates the directory that target will be in if it doesn't exist.

            File.WriteAllText(file.FullName, content);
        }
        catch(System.Exception e){
            Debug.LogError($"{e.Message}, {e.GetType()}");

            OpenFailedPanel();
            FailedPanelMsgText.text = $"Some error occurred during saving, {e.Message} : {e.GetType()}";
            return;
        }
    }

    public void LoadData(){
        try
        {
            string jsonContent = File.ReadAllText(SaveDataPath);
            DataBuffer = JsonUtility.FromJson<PlayerData>(jsonContent);
        }
        catch(System.Exception e)
        {
            Debug.LogError($"{e.Message}, {e.GetType()}");
            OpenFailedPanel();
            FailedPanelMsgText.text = $"Some error occurred during loading, {e.Message} : {e.GetType()}";
            return;
        }

        GlobalVariables.Instance.FrameRate = DataBuffer.FrameRate;
        GlobalVariables.Instance.EXP = DataBuffer.EXP;

        GlobalVariables.Instance.LoadKeys(DataBuffer);

        GlobalVariables.Instance.allPossessionList.Load(DataBuffer);

    }

    public bool ExistSavedGame()
    {
        try
        {
            FileStream fs = File.Open(SaveDataPath, FileMode.Open);
            fs.Close();
        }
        catch(FileNotFoundException)
        {
            return false;
        }
        catch(System.Exception e)
        {
            Debug.LogError($"{e.Message}, {e.GetType()}");
            return false;
        }

        return true;
    }

    public void CloseFailedPanel(){
        UIStack.Instance.RemovePanel(FailedPid);
    }
    void OpenFailedPanel(){
        FailedPid = UIStack.Instance.NewPanel(() =>{
            FailedPanel.SetActive(false);
        });
        FailedPanel.SetActive(true);
    }

    public void OpenConfirmPanel(){
        ConfirmPid = UIStack.Instance.NewPanel(() =>{
            ConfirmPanel.SetActive(false);
        });
        Debug.Log($"comfirm pid is {ConfirmPid}");
        ConfirmPanel.SetActive(true);
    }
    [ContextMenu("Close Confirm Menu")]
    public void CloseConfirmPanel(){
        UIStack.Instance.RemovePanel(ConfirmPid);
    }
}