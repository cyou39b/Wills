using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

// 讓Camera的移動變smooth
[RequireComponent(typeof(Camera))]
public class RbCameraMovement: MonoBehaviour{
    public float SmoothTime;
    public Vector3 Offset = new Vector3(0.0f, 0.0f, -10.0f);
    private Vector3 velocity = Vector3.zero;
    public float MaxX, MaxY;

    // [Range(0.0f, 1.0f)]
    // public float state = 0.0f; // 0.0f -> rb, 1.0f -> mid point
    public Jack Player;
    private Vector3 rbOffset = Vector3.zero;
    private Vector3 rbOffsetVelocity = Vector3.zero;
    public float RbOffestSmoothTime;

    public static Dictionary<Transform, float> Enemys = new Dictionary<Transform, float>();

    private Camera cam;
    public float CameraSizeSmoothTime;
    public float CameraSizeSmoothTimeFast;
    private float cameraSizeVelocity = 0.0f;

    void Start()
    {
        transform.position = Player.transform.position + Offset;
        cam = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        Vector3 lv = Player.rb.linearVelocity;
        lv.x = Mathf.Clamp(lv.x, -MaxX, MaxX);
        lv.y = Mathf.Clamp(lv.y, -MaxY, MaxY);

        if(lv.x != 0.0f || lv.y != 0.0)
        {
            rbOffset = Vector3.SmoothDamp(
                rbOffset,
                lv,
                ref rbOffsetVelocity,
                RbOffestSmoothTime
            );
        }

        Vector3 targetPos;
        float targetSize;
        if(Enemys.Count > 0)
        {
            targetPos = Player.transform.position + Offset * (cam.orthographicSize / 5.0f) + rbOffset;
            targetSize = CalculateProjectionSize(targetPos);
        }
        else
        {
            targetPos = Player.transform.position + Offset + rbOffset;
            targetSize = minProjectionSize;
        }

        cam.orthographicSize = Mathf.SmoothDamp(
            cam.orthographicSize,
            targetSize,
            ref cameraSizeVelocity,
            (targetSize > cam.orthographicSize) ? CameraSizeSmoothTimeFast : CameraSizeSmoothTime
        );

        transform.position = Vector3.SmoothDamp( // Unity's builtin function SmoothDamp do the calculations for us
            transform.position,
            targetPos,
            ref velocity,
            SmoothTime
        );
    }

    private const float minProjectionSize = 5.75f;
    private const float yToxAspectRatio = 16.0f / 9.0f;
    private const float xToyAspectRatio = 9.0f / 16.0f;
    private float CalculateProjectionSize(Vector3 pos)
    {
        float xMinProjectionSize = 0.0f;
        float yMinProjectionSize = 0.0f;
        foreach(KeyValuePair<Transform, float> kv in Enemys)
        {
            Vector3 target = kv.Key.transform.position - pos;
            xMinProjectionSize = Mathf.Max(xMinProjectionSize, Mathf.Abs(target.x) * xToyAspectRatio);
            yMinProjectionSize = Mathf.Max(yMinProjectionSize, Mathf.Abs(target.y));
        }
        float ans = Mathf.Max(xMinProjectionSize, yMinProjectionSize);
        return Mathf.Max(ans + 0.6f, minProjectionSize);
    }

    private Vector3 CalculateEnemyMidPoint()
    {
        Vector3 ans = Vector3.zero;
        float cnt = 0.0f;
        foreach(KeyValuePair<Transform, float> kv in Enemys)
        {
            if(kv.Key == null){cnt--;continue;}
            ans += kv.Key.position * kv.Value;
            cnt += kv.Value;
        }
        return ans / cnt;
    }
    public void Shake(float power) {}
}