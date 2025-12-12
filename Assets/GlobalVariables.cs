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
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        QualitySettings.vSyncCount = 0; // Disable VSync to use targetFrameRate
        Application.targetFrameRate = GlobalVariables.Instance.FrameRate;
    }
    public int FrameRate = 60;
    public Key JumpKey = Key.Space;
    public Key MoveLeftKey = Key.A;
    public Key MoveRightKey = Key.D;
    public Key InteractKey = Key.S;
}
