using UnityEngine;
using UnityEngine.InputSystem;

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
            QualitySettings.vSyncCount = 0; // Disable VSync to use targetFrameRate
            Application.targetFrameRate = GlobalVariables.Instance.FrameRate;
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
    public Key PickUpMine = Key.P;
    public Key FindMine = Key.N;

    // Some useful constants
    public const float PI = Mathf.PI;
    public const float NPI = - Mathf.PI;
    public const float HalfPI = PI / 2;
    public const float HalfNPI = NPI / 2;

    // Defined layers
    public const int PlayerLayer = 7;
    public const int PlayerLayerMask = 1 << PlayerLayer;
    public const int EnemyLayer = 8;
    public const int EnemyLayerMask = 1 << EnemyLayer;
    public const int GroundLayer = 9;
    public const int GroundLayerMask = 1 << GroundLayer;
}