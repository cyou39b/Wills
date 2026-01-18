using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MachineLogic))]
public class MachineLogic_Inspector : Editor
{
    public virtual void OnSceneGUI()
    {
        MachineLogic targetScript = (MachineLogic) target;
        targetScript.ButtomRight = Handles.PositionHandle(
            targetScript.ButtomRight,
            Quaternion.identity
        );
        Handles.Label(targetScript.ButtomRight, "Buttom Right");
        targetScript.TopLeft = Handles.PositionHandle(
            targetScript.TopLeft,
            Quaternion.identity
        );
        Handles.Label(targetScript.TopLeft, "Top Left");
    }
}
