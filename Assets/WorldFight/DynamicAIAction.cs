using System;
using NavMeshPlus.Components;
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
    public NavMeshLink Link;
    
    [Tooltip("Wills1 will jump after [0, TimeRange] fixed updates when it enters jumpRange.")]
    public int TimeRange;
    [Tooltip("The highest point of the jump curve")]
    public float JumpHighestPoint;
    public float StartZoneLeft = -2.0f;
    public float StartZoneRight = 11.0f;
    public float EndZoneLeft = 0.0f;
    public float EndZoneRight = 1.0f;


    [Tooltip("Speed on X direction.")]
    public float XSpeed;
    [Tooltip("Where to switch into walk off edge intent.")]
    public float StartX;
    [Tooltip("Keep walking until the GameObject cross this line.")]
    public float TargetX;

    public bool inRange(float x)
    {
        return Type switch
        {
            ActionType.Jump => StartZoneLeft <= x && x <= StartZoneRight,
            ActionType.WalkOffEdge => StartX < TargetX ? StartX <= x : StartX >= x,
            _ => false
        };
    }
}