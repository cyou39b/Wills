using UnityEngine;

public class DynamicAIAction : MonoBehaviour
{
    public enum ActionType
    {
        Jump,
        WalkOffEdge
    }
    [Tooltip("What action is this block want wills1 to do?")]
    public ActionType Type;

    [Tooltip("The link this data is describing.")]
    public GameObject Link;
    [Tooltip("Speed on X direction.")]
    public float XSpeed;
    
    [Tooltip("Da speed that wills1 jump.")]
    public float JumpSpeed;
    [Tooltip("Make the jump speed be in range [Speed - SpeedRange, Speed + SpeedRange].\n Sometimes a little randomness just makes your game better. (I suppose)")]
    public float JumpSpeedRange;
    [Tooltip("Wills1 will jump after [0, TimeRange] fixed updates.")]
    public int TimeRange;

    [Tooltip("Keep walking until the GameObject cross this line.")]
    public float TargetX;
}