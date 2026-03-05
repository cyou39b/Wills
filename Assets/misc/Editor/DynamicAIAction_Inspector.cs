using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DynamicAIAction))]
public class DynamicAIAction_Inspector : Editor {

    private SerializedProperty typeProp;
    private SerializedProperty linkProp;
    private SerializedProperty xSpeedProp;

    // ---- if type == Jump ----
    private SerializedProperty jumpSpeedProp;
    private SerializedProperty jumpSpeedRangeProp;
    private SerializedProperty jumpRangeLeft;
    private SerializedProperty jumpRangeRight;
    private SerializedProperty timeRangeProp;

    // ---- if type == walk ----
    private SerializedProperty targetXProp;

    private void OnEnable()
    {
        typeProp = serializedObject.FindProperty("Type");
        linkProp = serializedObject.FindProperty("Link");
        xSpeedProp = serializedObject.FindProperty("XSpeed");
        jumpSpeedProp = serializedObject.FindProperty("JumpSpeed");
        jumpSpeedRangeProp = serializedObject.FindProperty("JumpSpeedRange");
        jumpRangeLeft = serializedObject.FindProperty("JumpRangeLeft");
        jumpRangeRight = serializedObject.FindProperty("JumpRangeRight");
        timeRangeProp = serializedObject.FindProperty("TimeRange");
        targetXProp = serializedObject.FindProperty("TargetX");
    }

    public override void OnInspectorGUI()
    {
        DynamicAIAction script = (DynamicAIAction)target;

        serializedObject.Update();
        EditorGUILayout.LabelField("Action Settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(typeProp);
        if(EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        DynamicAIAction.ActionType selectedType = (DynamicAIAction.ActionType)typeProp.enumValueIndex;

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(linkProp);
        EditorGUILayout.PropertyField(xSpeedProp);

        switch(selectedType)
        {
            case DynamicAIAction.ActionType.Jump:
                EditorGUILayout.PropertyField(jumpSpeedProp);
                EditorGUILayout.PropertyField(jumpSpeedRangeProp);
                EditorGUILayout.PropertyField(jumpRangeLeft);
                EditorGUILayout.PropertyField(jumpRangeRight);
                EditorGUILayout.PropertyField(timeRangeProp);
                break;
            case DynamicAIAction.ActionType.WalkOffEdge:
                EditorGUILayout.PropertyField(targetXProp);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    public virtual void OnSceneGUI()
    {
        DynamicAIAction script = (DynamicAIAction)target;
        
        if(script.Type == DynamicAIAction.ActionType.WalkOffEdge)
        {
            Handles.DrawLine(
                new Vector3(script.TargetX, 100.0f, 0.0f),
                new Vector3(script.TargetX, -100.0f, 0.0f),
                1.2f
            );
        }
        else
        {
            Handles.DrawLine(
                new Vector3(script.JumpRangeLeft, 100.0f, 0.0f),
                new Vector3(script.JumpRangeLeft, -100.0f, 0.0f),
                1.2f
            );
            Handles.DrawLine(
                new Vector3(script.JumpRangeRight, 100.0f, 0.0f),
                new Vector3(script.JumpRangeRight, -100.0f, 0.0f),
                1.2f
            );
        }
    }
}