using System;
using System.Drawing.Drawing2D;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.XR;

[CustomEditor(typeof(DynamicAIAction))]
public class DynamicAIAction_Inspector : Editor {

    private SerializedProperty typeProp;
    private SerializedProperty linkProp;

    // ---- if type == Jump ----
    private SerializedProperty timeRangeProp;
    private SerializedProperty jumpHighestPointProp;

    // ---- if type == walk ----
    private SerializedProperty xSpeedProp;
    private SerializedProperty startXProp;
    private SerializedProperty targetXProp;


    private void OnEnable()
    {
        typeProp = serializedObject.FindProperty("Type");
        linkProp = serializedObject.FindProperty("Link");
        timeRangeProp = serializedObject.FindProperty("TimeRange");
        jumpHighestPointProp = serializedObject.FindProperty("JumpHighestPoint");
        xSpeedProp = serializedObject.FindProperty("XSpeed");
        startXProp = serializedObject.FindProperty("StartX");
        targetXProp = serializedObject.FindProperty("TargetX");
    }

    float v_x, v_y;
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

        switch(selectedType)
        {
            case DynamicAIAction.ActionType.Jump:
                EditorGUILayout.PropertyField(timeRangeProp);
                EditorGUILayout.PropertyField(jumpHighestPointProp);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if(GUILayout.Button("Refresh Jump path", GUILayout.Width(150)))
                {
                    highestPoint.y = 114.514f;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if(!string.IsNullOrEmpty(err)) {EditorGUILayout.HelpBox(err, MessageType.Error);}
                GUI.enabled = false;
                EditorGUILayout.Vector2Field("Initial velocity", new Vector2(v_x, v_y));
                GUI.enabled = true;
                break;
            case DynamicAIAction.ActionType.WalkOffEdge:
                EditorGUILayout.PropertyField(xSpeedProp);
                EditorGUILayout.PropertyField(startXProp);
                EditorGUILayout.PropertyField(targetXProp);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    int selectedPoint;

    string err = "";
    Vector3 highestPoint = new Vector3(0.0f, 114.514f, 0.0f); // set y to a oddly specific value so that it will always get updated on the first onSceneGUI run
    Vector3[] dots = new Vector3[0];
    Vector3[] leftCurveDots = new Vector3[0];
    Vector3[] rightCurveDots = new Vector3[0];
    public virtual void OnSceneGUI()
    {
        DynamicAIAction script = (DynamicAIAction)target;
        Vector3 linkStartPoint = script.Link.startPoint;
        Vector3 linkEndPoint = script.Link.endPoint;
        
        if(script.Type == DynamicAIAction.ActionType.WalkOffEdge)
        {
            Handles.DrawLine(
                new Vector3(script.TargetX, 100.0f, 0.0f),
                new Vector3(script.TargetX,-100.0f, 0.0f),
                1.2f
            );
            Handles.color = Color.black;
            Handles.DrawLine(
                new Vector3(script.StartX, 100.0f, 0.0f),
                new Vector3(script.StartX,-100.0f, 0.0f),
                1.2f
            );
        }
        else
        {
            bool reCalculate = highestPoint.y == 114.514f;

            Handles.color = Color.gold;

            float startRangeY = (script.Link.transform.rotation * linkStartPoint + script.Link.transform.position).y;
            // startZoneLeft
            {
                float handleSize = HandleUtility.GetHandleSize(new Vector3(script.StartZoneLeft, startRangeY, 0.0f)) * 0.1f;
                if(selectedPoint == 0)
                {
                    EditorGUI.BeginChangeCheck();
                    Handles.CubeHandleCap(
                        0,
                        new Vector3(script.StartZoneLeft, startRangeY, 0.0f),
                        Quaternion.identity,
                        handleSize,
                        Event.current.type
                    );
                    float pos = Handles.PositionHandle(
                        new Vector3(script.StartZoneLeft, startRangeY, 0.0f),
                        script.transform.rotation
                    ).x;
                    if(EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(script, "Move range");
                        script.StartZoneLeft = Mathf.Min(pos, script.StartZoneRight);
                        reCalculate = true;
                    }
                }
                else
                {
                    if(
                        Handles.Button(
                            new Vector3(script.StartZoneLeft, startRangeY, 0.0f),
                            Quaternion.identity,
                            handleSize,
                            handleSize,
                            Handles.CubeHandleCap
                        )
                    ) {
                        selectedPoint = 0;
                    }
                }
            }

            // startZoneRight
            {
                float handleSize = HandleUtility.GetHandleSize(new Vector3(script.StartZoneRight, startRangeY, 0.0f)) * 0.1f;
                if(selectedPoint == 1)
                {
                    EditorGUI.BeginChangeCheck();
                    Handles.CubeHandleCap(
                        0,
                        new Vector3(script.StartZoneRight, startRangeY, 0.0f),
                        Quaternion.identity,
                        handleSize,
                        Event.current.type
                    );
                    float pos = Handles.PositionHandle(
                        new Vector3(script.StartZoneRight, startRangeY, 0.0f),
                        script.transform.rotation
                    ).x;
                    if(EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(script, "Move range");
                        script.StartZoneRight = Mathf.Max(pos, script.StartZoneLeft);
                        reCalculate = true;
                    }
                }
                else
                {
                    if(
                        Handles.Button(
                            new Vector3(script.StartZoneRight, startRangeY, 0.0f),
                            Quaternion.identity,
                            handleSize,
                            handleSize,
                            Handles.CubeHandleCap
                        )
                    ) {
                        selectedPoint = 1;
                    }
                }
            }

            Handles.DrawLine(
                new Vector3(script.StartZoneLeft, startRangeY, 0.0f),
                new Vector3(script.StartZoneRight, startRangeY, 0.0f),
                0.15f
            );

            float endRangeY = (script.Link.transform.rotation * linkEndPoint + script.Link.transform.position).y;
            
            Handles.color = Color.navyBlue;
            // endZoneLeft
            {
                float handleSize = HandleUtility.GetHandleSize(new Vector3(script.EndZoneLeft, endRangeY, 0.0f)) * 0.1f;
                if(selectedPoint == 2)
                {
                    EditorGUI.BeginChangeCheck();
                    Handles.CubeHandleCap(
                        0,
                        new Vector3(script.EndZoneLeft, endRangeY, 0.0f),
                        Quaternion.identity,
                        handleSize,
                        Event.current.type
                    );
                    float pos = Handles.PositionHandle(
                        new Vector3(script.EndZoneLeft, endRangeY, 0.0f),
                        script.transform.rotation
                    ).x;
                    if(EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(script, "Move range");
                        script.EndZoneLeft = Mathf.Min(pos, script.EndZoneRight);
                        reCalculate = true;
                    }
                }
                else
                {
                    if(
                        Handles.Button(
                            new Vector3(script.EndZoneLeft, endRangeY, 0.0f),
                            Quaternion.identity,
                            handleSize,
                            handleSize,
                            Handles.CubeHandleCap
                        )
                    ) {
                        selectedPoint = 2;
                    }
                }
            }

            // endZoneRight
            {
                float handleSize = HandleUtility.GetHandleSize(new Vector3(script.EndZoneRight, endRangeY, 0.0f)) * 0.1f;
                if(selectedPoint == 3)
                {
                    EditorGUI.BeginChangeCheck();
                    Handles.CubeHandleCap(
                        0,
                        new Vector3(script.EndZoneRight, endRangeY, 0.0f),
                        Quaternion.identity,
                        handleSize,
                        Event.current.type
                    );
                    float pos = Handles.PositionHandle(
                        new Vector3(script.EndZoneRight, endRangeY, 0.0f),
                        script.transform.rotation
                    ).x;
                    if(EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(script, "Move range");
                        script.EndZoneRight = Mathf.Max(pos, script.EndZoneLeft);
                        reCalculate = true;
                    }
                }
                else
                {
                    if(
                        Handles.Button(
                            new Vector3(script.EndZoneRight, endRangeY, 0.0f),
                            Quaternion.identity,
                            handleSize,
                            handleSize,
                            Handles.CubeHandleCap
                        )
                    ) {
                        selectedPoint = 3;
                    }
                }
            }

            Handles.DrawLine(
                new Vector3(script.EndZoneLeft, endRangeY, 0.0f),
                new Vector3(script.EndZoneRight, endRangeY, 0.0f),
                0.15f
            );

            // highestPoint
            {
                Handles.color = Color.white;
                float handleSize = HandleUtility.GetHandleSize(highestPoint) * 0.1f;
                if(selectedPoint == 4)
                {
                    EditorGUI.BeginChangeCheck();
                    Handles.CubeHandleCap(
                        0,
                        highestPoint,
                        Quaternion.identity,
                        handleSize,
                        Event.current.type
                    );
                    float newY= Handles.PositionHandle(
                        new Vector3(highestPoint.x, script.JumpHighestPoint, 0.0f), 
                        Quaternion.identity
                    ).y;
                    if(EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(script, "Move highest point");
                        script.JumpHighestPoint = newY;
                        reCalculate = true;
                    }
                }
                else
                {
                    if(
                        Handles.Button(
                            highestPoint,
                            Quaternion.identity,
                            handleSize,
                            handleSize,
                            Handles.CubeHandleCap
                        )
                    ) {
                        selectedPoint = 4;
                    }
                }
            }

            if(reCalculate)
            {
                leftCurveDots = FindDots(
                    new Vector3(script.StartZoneLeft, startRangeY, 0.0f),
                    new Vector3(script.EndZoneRight, endRangeY, 0.0f),
                    script.JumpHighestPoint
                );
                rightCurveDots = FindDots(
                    new Vector3(script.StartZoneRight, startRangeY, 0.0f),
                    new Vector3(script.EndZoneLeft, endRangeY, 0.0f),
                    script.JumpHighestPoint
                );
                dots = FindDots(
                    new Vector3((script.StartZoneLeft + script.StartZoneRight) / 2, startRangeY, 0.0f),
                    new Vector3((script.EndZoneLeft + script.EndZoneRight) / 2, endRangeY, 0.0f),
                    script.JumpHighestPoint
                );
            }
            
            Handles.color = Color.green;
            Handles.DrawPolyLine(leftCurveDots);
            Handles.color = Color.blueViolet;
            Handles.DrawPolyLine(rightCurveDots);
            Handles.color = Color.gold;
            Handles.DrawPolyLine(dots);
        }
    }

    private Vector3[] FindDots(Vector3 startPoint, Vector3 endPoint, float highestPointY)
    {
        (string op_err, JumpFunction? op_jf) = MathUtil.CalculateJumpCurve(
            startPoint,
            endPoint,
            highestPointY
        );
        JumpFunction jf;

        if(op_jf == null)
        {
            err = op_err;
            return new Vector3[0];
        }
        else {jf = op_jf.Value; err = null;}

        v_x = jf.xt.velocity; v_y = jf.yt.b;
        highestPoint = jf.vertex;
        float stepSize = 0.05f;
        int stepCnt = (int)Mathf.Ceil(jf.end_t / stepSize);
        Vector3[] dots = new Vector3[stepCnt + 1];
        for(int i=0;i<stepCnt;i++)
        {
            float t = stepSize * i;
            dots[i] = new Vector3(jf.xt.On_t(t), jf.yt.On_t(t), 0.0f);
        }
        dots[^1] = endPoint;
        return dots;
    }
}