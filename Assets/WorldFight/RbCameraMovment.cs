using UnityEngine;

// 讓Camera的移動變smooth

public class JackCameraMove : MonoBehaviour{
    public float state = 0.0f; // 0.0f -> rb, 1.0f -> mid point
    public Jack Player;
    public static Transform enemy = null;
    public Vector3 Offset = new Vector3(0.0f, 0.0f, -10.0f);
    public float RbOffestSmoothTime;
    public float SmoothTime;
    
    private Vector3 velocity = Vector3.zero;
    private Vector3 rbOffset = Vector3.zero;
    private Vector3 rbOffsetVelocity = Vector3.zero;
    public float MaxX, MaxY;

    void Start()
    {
        transform.position = Player.transform.position + Offset;
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

        Vector3 midPoint = Vector3.zero;
        if(enemy != null)
        {
            midPoint = Player.transform.position - enemy.position;
        }

        Vector3 targetPos = (Player.transform.position + Offset + rbOffset) * (1.0f-state) + midPoint * state;

        transform.position = Vector3.SmoothDamp( // Unity's builtin function SmoothDamp do the calculations for us
            transform.position,
            targetPos,
            ref velocity,
            SmoothTime
        );
    }

    public void Shake(float power) {}
}