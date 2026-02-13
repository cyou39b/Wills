using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// This class stores global variables, uses Singleton design pattern.
// To get/set a variable: use "GlobalVariables.Instace.<FieldName>".
public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables Instance {get; private set;} = null; // The singleton instance, initialize as null

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
            SceneManager.activeSceneChanged += OnSceneChange;
            QualitySettings.antiAliasing = 0;
            QualitySettings.vSyncCount = 0; // Disable VSync to use targetFrameRate
            Application.targetFrameRate = GlobalVariables.Instance.FrameRate;
        }

    }

    public void OnSceneChange(Scene prev, Scene curr)
    {
        string currSceneName = curr.name;
        switch(currSceneName)
        {
            case "MainMenu":
            case "MainScene":
            case "LoadSceneBuffer":
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
    }

    // ----------------- variables below ------------------------
    public int FrameRate = 60;
    public int NumMines = 0;

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
    
    // Defined layers
    public const int PlayerLayer = 7;
    public const int PlayerLayerMask = 1 << PlayerLayer;
    public const int EnemyLayer = 8;
    public const int EnemyLayerMask = 1 << EnemyLayer;
    public const int GroundLayer = 9;
    public const int GroundLayerMask = 1 << GroundLayer;
    public const int WallLayer = 10;
    public const int WallLayerMask = 1 << WallLayer;
}