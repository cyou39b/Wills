using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData{
    // basic data in game 
    public int Mine;
    public string Scene;
    public float xPos;
    public float yPos;
    public float zPos;
    public List<int> BackpackIndex; //save the thing in backpack
    public List<int> shop;
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
//I haven't do the shop part ......
public class DataSave : MonoBehaviour{
    readonly string path = Application.persistentDataPath + "/save.json";
    public static DataSave Instance{get;private set;} = null;
    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this.gameObject);
        }
        else{
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
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

        string content = JsonUtility.ToJson(data);
        File.WriteAllText(path,content);
    }
    public void LoadData(){ //TODO
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

        LoadSceneManager.NextScene = deserializeData.Scene;
        SceneManager.LoadScene("LoadSceneBuffer");
        if(SceneManager.GetActiveScene().name == deserializeData.Scene){ //TODO
            GameObject Player = GameObject.FindWithTag("Player");
            if(Player == null){ 
                Debug.LogError("Couldn't find player");
                SceneManager.LoadScene("MainMenu");
            }
            Player.transform.position = new Vector3(deserializeData.xPos,deserializeData.yPos,deserializeData.zPos);
        }
    }
}