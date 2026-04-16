using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class PlayerData{
    // basic data in game 
    public int Mine;
    public string Scene;
    public float xPos;
    public float yPos;
    public float zPos;
    public List<int> BackpackIndex; //TODO
    //record Player setting
    public int FrameRate;
    public string Jump;
    public string MoveLeft;
    public string MoveRight;
    public string Up;
    public string Down;
    public string Interact;
    public string Attack;
    public string FindMine;
}
public class DataSave : MonoBehaviour{
    string path;
    public static DataSave Instance{get;private set;} = null;
    public GameObject FailUIPrefab;
    void Awake(){
        path = Application.persistentDataPath + "/save.json";
        if (Instance != null && Instance != this){
            Destroy(this.gameObject);
            return;
        }
        else{
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        if(FailUIPrefab.transform.Find("OK").TryGetComponent<Button>(out Button btn)){
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(CloseFailedUI);
        }
    }
    public void SaveData(GameObject Player){
        PlayerData data = new PlayerData();
        data.Mine = GlobalVariables.Instance.NumMines;
        data.Scene = SceneManager.GetActiveScene().name;
        data.xPos = Player.transform.position.x;
        data.yPos = Player.transform.position.y;
        data.zPos = Player.transform.position.z;

        data.Attack = GlobalVariables.Instance.AttackKey.ToString();
        data.Down = GlobalVariables.Instance.DownKey.ToString();
        data.FindMine = GlobalVariables.Instance.FindMineKey.ToString();
        data.FrameRate = GlobalVariables.Instance.FrameRate;
        data.Interact = GlobalVariables.Instance.InteractKey.ToString();
        data.Jump = GlobalVariables.Instance.JumpKey.ToString();
        data.MoveLeft = GlobalVariables.Instance.MoveLeftKey.ToString();
        data.MoveRight = GlobalVariables.Instance.MoveRightKey.ToString();
        data.Up = GlobalVariables.Instance.UpKey.ToString();

        foreach(PossessionItems item in GlobalVariables.Instance.possession){
            data.BackpackIndex.Add(item.index);
        }

        string content = JsonUtility.ToJson(data);
        try{
            File.WriteAllText(path,content);
        }
        catch(Exception e){
            Debug.LogError(e);
            //show Failed UI
            if(FailUIPrefab.transform.Find("Msg").TryGetComponent<Text>(out Text txt3)){
                txt3.text = e.Message;
            }
            FailUIPrefab.SetActive(true);
            return;
        }
    }
    public void LoadData(){
        if (!File.Exists(path)){
            Debug.LogWarning("Couldn't find the save data");
            return;
        }
        string jsonContent = File.ReadAllText(path);
        PlayerData deserializeData = JsonUtility.FromJson<PlayerData>(jsonContent);

        GlobalVariables.Instance.AttackKey = (Key)System.Enum.Parse(typeof(Key),deserializeData.Attack);
        GlobalVariables.Instance.DownKey = (Key)System.Enum.Parse(typeof(Key),deserializeData.Down);
        GlobalVariables.Instance.FindMineKey = (Key)System.Enum.Parse(typeof(Key),deserializeData.FindMine);
        GlobalVariables.Instance.FrameRate = deserializeData.FrameRate;
        GlobalVariables.Instance.InteractKey = (Key)System.Enum.Parse(typeof(Key),deserializeData.Interact);
        GlobalVariables.Instance.JumpKey = (Key)System.Enum.Parse(typeof(Key),deserializeData.Jump);
        GlobalVariables.Instance.MoveLeftKey = (Key)System.Enum.Parse(typeof(Key),deserializeData.MoveLeft);
        GlobalVariables.Instance.MoveRightKey = (Key)System.Enum.Parse(typeof(Key),deserializeData.MoveRight);
        GlobalVariables.Instance.UpKey = (Key)System.Enum.Parse(typeof(Key),deserializeData.Up);

        foreach(int index in deserializeData.BackpackIndex){
            GlobalVariables.Instance.possession.Add(GlobalVariables.Instance.AllPossession[index]);
        }

        LoadSceneManager.NextScene = deserializeData.Scene;
        SceneManager.LoadScene("LoadSceneBuffer");
        if(SceneManager.GetActiveScene().name == deserializeData.Scene){ //Fixed
            GameObject Player = GameObject.FindWithTag("Player");
            if(Player == null){ 
                Debug.LogError("Couldn't find player");
                SceneManager.LoadScene("MainMenu");
                return;
            }
            else{
                Player.transform.position = new Vector3(deserializeData.xPos,deserializeData.yPos,deserializeData.zPos);
            }
        }
    }
    void CloseFailedUI(){
        FailUIPrefab.SetActive(false);
    }
}