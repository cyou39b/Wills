using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using System.Collections;

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

    private Vector3 pos = Vector3.zero;

    public static Dictionary<Transform, Rigidbody2D> Enemys = new Dictionary<Transform, Rigidbody2D>();

    private Camera cam;
    public float CameraSizeSmoothTime;
    public float CameraSizeSmoothTimeFast;
    private float cameraSizeVelocity = 0.0f;

    void Start()
    {
        pos = Player.transform.position + Offset;
        transform.position = pos;
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

        pos = Vector3.SmoothDamp( // Unity's builtin function SmoothDamp do the calculations for us
            pos,
            targetPos,
            ref velocity,
            SmoothTime
        );

        transform.position = MathUtil.AddVectors(pos, screenShakeOffset);
    }

    private const float minProjectionSize = 5.75f;
    private const float yToxAspectRatio = 16.0f / 9.0f;
    private const float xToyAspectRatio = 9.0f / 16.0f;
    private float CalculateProjectionSize(Vector3 pos)
    {
        Vector2 minProjectionSizeVec = new Vector2(0.0f, 0.0f);
        foreach(KeyValuePair<Transform, Rigidbody2D> kv in Enemys)
        {
            Vector3 target = kv.Key.transform.position - pos;

            if(kv.Value != null)
            {
                Vector2 velocity = kv.Value.linearVelocity;

                // NOTE: Make the camera move 15 frames(~= 0.3 seconds) ahead of the moving object
                if(MathUtil.SameSign(target.x, velocity.x))
                {
                    target.x += velocity.x * Time.fixedDeltaTime * 15.0f;
                }

                if(MathUtil.SameSign(target.y, velocity.y))
                {
                    target.y += velocity.y * Time.fixedDeltaTime * 15.0f;
                }
            }

            minProjectionSizeVec.x = Mathf.Max(minProjectionSizeVec.x, Mathf.Abs(target.x) * xToyAspectRatio);
            minProjectionSizeVec.y = Mathf.Max(minProjectionSizeVec.y, Mathf.Abs(target.y));
        }
        float ans = Mathf.Max(minProjectionSizeVec.x, minProjectionSizeVec.y);
        return Mathf.Max(ans + 0.6f, minProjectionSize);
    }

    public void Shake(float power, float duration)
    {
        StartCoroutine(screenShakeIt(power, duration));
    }

    private Vector2 screenShakeOffset = Vector2.zero;
    IEnumerator screenShakeIt(float power, float duration)
    {
        float recoverySpeed = 1.0f / duration;
        float trauma = 1.0f;

        while(trauma > 0.0f)
        {
            trauma = Mathf.Clamp01(trauma - recoverySpeed * Time.deltaTime);

            float shake = Mathf.Pow(trauma, 2.0f);

            screenShakeOffset.x = power * shake * (Mathf.PerlinNoise(Time.time * 25.0f, 0.0f) * 2.0f - 1.0f);
            screenShakeOffset.y = power * shake * (Mathf.PerlinNoise(0.0f, Time.time * 25.0f) * 2.0f - 1.0f);
            float angle = power * shake * (Mathf.PerlinNoise(Time.time * 20.0f, Time.time * 20.0f) * 2.0f - 1.0f);
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
            yield return null;
        }

        screenShakeOffset = Vector2.zero;
        transform.rotation = Quaternion.identity;
    }
}