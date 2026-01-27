using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MachineLogic))]
public class MachineLogic_Inspector : Editor
{
    private static readonly Quaternion turn180 = Quaternion.Euler(0.0f, 0.0f, 180.0f);
    private static readonly Color[] rectColors =
    {
        Color.green,
        Color.cyan,
        Color.magenta,
        Color.yellow,
        Color.red,
        Color.blue,
        Color.white,
    };
    public virtual void OnSceneGUI()
    {
        MachineLogic script = (MachineLogic) target;

        script.ButtomRight = Handles.PositionHandle(
            script.ButtomRight,
            Quaternion.identity
        );
        Handles.Label(script.ButtomRight, "Buttom Right");

        script.TopLeft = Handles.PositionHandle(
            script.TopLeft,
            Quaternion.identity
        );
        Handles.Label(script.TopLeft, "Top Left");
        
        for(int i=0;i<script.Rects.Length;i++)
        {
            Rect curr = script.Rects[i];

            Handles.DrawSolidRectangleWithOutline(
                curr,
                Color.clear,
                rectColors[i%rectColors.Length]
            );

            Vector3 newButtomLeft = Handles.PositionHandle(
                new Vector3(curr.xMin, curr.yMin),
                Quaternion.identity
            );
            curr.xMin = newButtomLeft.x;
            curr.yMin = newButtomLeft.y;

            Vector3 newTopRight = Handles.PositionHandle(
                new Vector3(curr.xMax, curr.yMax),
                turn180
            );
            curr.xMax = newTopRight.x;
            curr.yMax = newTopRight.y;

            script.Rects[i] = curr;
        }
    }
}
