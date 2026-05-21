using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// This class stores global variables, uses Singleton design pattern.
// To get/set a variable: use "GlobalVariables.Instace.<FieldName>".
public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables Instance {get; private set;} = null; // The singleton instance, initialize as null
    private static bool firstLoad = true;
// The function "Awake" initialize "GlobalVariables.Instance"
// You MUST somehow make sure the "Awake" function gets called \
// at least once or "GlobalVariables.Instance" will be null \
// and everything fails.
    public void Awake()
    {
        if(Instance != null && Instance != this)
        {
            // destroy this.gameObject if GlobalVariables.Instance existes
            Debug.Log("Global Variables Instance already exist.");
            Destroy(this.gameObject);
            return;
        }
        else
        {
            // set GlobalVariables.Instance to this if it doesn't existes.
            Debug.Log("Global Instance created");
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            if (firstLoad)
            {
                firstLoad = false;
                AudioListener.volume = 0.9f;
                SceneManager.activeSceneChanged += OnSceneChange;
                QualitySettings.antiAliasing = 0;
                QualitySettings.vSyncCount = 0; // Disable VSync to use targetFrameRate
                Application.targetFrameRate = GlobalVariables.Instance.FrameRate;
            }
        }

    }

    public void OnSceneChange(Scene prev, Scene curr)
    {
        Debug.Log($"Scene change: {prev.name} -> {curr.name}");

        string currSceneName = curr.name;
        switch(currSceneName)
        {
            case "LoadSceneBuffer":
                break;
            case "MainMenu":
            case "MainScene":
            case "WorldMining":
                QualitySettings.antiAliasing = 0;
                break;
            case "WorldFight":
                QualitySettings.antiAliasing = 1;
                break;
            default:
                Debug.LogWarning("OnSceneChange: unknown scene loaded.");
                break;
        }

        if(DataSave.DataBuffer != null &&
           DataSave.DataBuffer.PlayerPos != null && 
           !DataSave.DataBuffer.positionWasRead &&
           currSceneName == DataSave.DataBuffer.CurrentSceneName
        ) {
            DataSave.DataBuffer.positionWasRead = true;
            GameObject player = GameObject.Find("Player");
            if(player == null)
            {
                Debug.LogError("Current scene don't have player.");
            }

            player.transform.position = DataSave.DataBuffer.PlayerPos.Value;
        }
    }

    public bool isQuitting = false;
    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    public void LoadKeys(in PlayerData data)
    {
        try
        {
            Instance.AttackKey = (Key)System.Enum.Parse(typeof(Key),data.AttackKey);
            Instance.MoveRightKey = (Key)System.Enum.Parse(typeof(Key),data.MoveRightKey);
            Instance.DownKey = (Key)System.Enum.Parse(typeof(Key),data.DownKey);
            Instance.FindMineKey = (Key)System.Enum.Parse(typeof(Key),data.FindMineKey);
            Instance.JumpKey = (Key)System.Enum.Parse(typeof(Key),data.JumpKey);
            Instance.MoveLeftKey = (Key)System.Enum.Parse(typeof(Key),data.MoveLeftKey);
            Instance.InteractKey = (Key)System.Enum.Parse(typeof(Key),data.InteractKey);
            Instance.UpKey = (Key)System.Enum.Parse(typeof(Key),data.UpKey);
        }
        catch(System.Exception e)
        {
            Debug.LogError($"{e.Message}, {e.GetType()}");
        }
    }

    public void SaveKeys(PlayerData data)
    {
        data.InteractKey = Instance.InteractKey.ToString();
        data.FindMineKey = Instance.FindMineKey.ToString();
        data.JumpKey = Instance.JumpKey.ToString();
        data.AttackKey = Instance.AttackKey.ToString();
        data.MoveLeftKey = Instance.MoveLeftKey.ToString();
        data.MoveRightKey = Instance.MoveRightKey.ToString();
        data.UpKey = Instance.UpKey.ToString();
        data.DownKey = Instance.DownKey.ToString();
    }

    // ----------------- variables below ------------------------
    public int FrameRate = 60;
    public int NumMines = 0;
    public int EXP = 0;

    public Key JumpKey = Key.Space;
    public Key MoveLeftKey = Key.A;
    public Key MoveRightKey = Key.D;
    public Key InteractKey = Key.F;
    public Key AttackKey = Key.L;
    public Key UpKey = Key.W;
    public Key DownKey = Key.S;
    public Key PickUpMineKey => InteractKey;
    public Key FindMineKey = Key.N;

    public List<PossessionItems> possession;
    public PossessionItems[] AllPossession => allPossessionList.List;
    public AllPossessionList allPossessionList;
}

public class DefinedLayers 
{
    public const int PlayerLayer = 7;
    public const int PlayerLayerMask = 1 << PlayerLayer;
    public const int EnemyLayer = 8;
    public const int EnemyLayerMask = 1 << EnemyLayer;
    public const int GroundLayer = 9;
    public const int GroundLayerMask = 1 << GroundLayer;
    public const int WallLayer = 10;
    public const int WallLayerMask = 1 << WallLayer;
    public const int AttackLayer = 11;
    public const int AttackLayerMask = 1 << AttackLayer;
    public const int NavMeshLayer = 13;
    public const int NavMeshLayerMask = 1 << NavMeshLayer;
}