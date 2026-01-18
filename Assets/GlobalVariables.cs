using UnityEngine;
using UnityEngine.InputSystem;

// Class GlobalVariables stores global variables.
// To get/set a variable: use "GlobalVariables.Instace.<FieldName>".
// The function "Awake" initialize "GlobalVariables.Instance"
// You MUST somehow make sure the "Awake" function gets called \
// at least once or "GlobalVariables.Instance" will be null \
// and everything exploades.
public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables Instance{get; private set;}
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Debug.Log("Global Variables Instance already exist.");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Global Instance created");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        QualitySettings.vSyncCount = 0; // Disable VSync to use targetFrameRate
        Application.targetFrameRate = GlobalVariables.Instance.FrameRate;
    }
    public int FrameRate = 60;
    public int NumMines = 0;
    public Key JumpKey = Key.Space;
    public Key MoveLeftKey = Key.A;
    public Key MoveRightKey = Key.D;
    public Key InteractKey = Key.F;
    public Key AttackKey = Key.L;
    // using in mining 
    public Key UpKey = Key.W;
    public Key DownKey = Key.S;
    public Key PickUpMine = Key.P;
    public Key FindMine = Key.N;

    // Some useful numbers
    public const float PI = Mathf.PI;
    public const float NPI = - Mathf.PI;
    public const float HalfPI = PI / 2;
    public const float HalfNPI = NPI / 2;
}