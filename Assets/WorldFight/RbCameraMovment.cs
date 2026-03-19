using UnityEngine;

// 讓Camera的移動變smooth

public class JackCameraMove : MonoBehaviour{
    public Jack Player;
    public Vector3 Offset = new Vector3(0.0f, 0.0f, -10.0f);
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

        rbOffset = Vector3.SmoothDamp(
            rbOffset,
            lv,
            ref rbOffsetVelocity,
            SmoothTime
        );

        Vector3 targetPos = Player.transform.position + Offset + rbOffset;

        transform.position = Vector3.SmoothDamp( // Unity's builtin function SmoothDamp do the calculations for us
            transform.position,
            targetPos,
            ref velocity,
            SmoothTime
        );
    }

    public void Shake(float power) {}
}