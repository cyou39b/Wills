using UnityEngine;

// 讓Camera的移動變smooth

public class CameraMove : MonoBehaviour{
    public Transform Target;
    public Rigidbody2D TargetRB;
    public Vector3 Offset = new Vector3(0.0f, 0.0f, -10.0f);
    public float SmoothTime;
    
    private Vector3 velocity = Vector3.zero;
    public Vector3 rbOffset = Vector3.zero;
    private Vector3 rbOffsetVelocity = Vector3.zero;
    void Start()
    {
        transform.position = Target.position + Offset;
    }

    void FixedUpdate()
    {
        Vector3 targetPos = Target.position + Offset;

        if(TargetRB != null)
        {
            Vector2 lv = TargetRB.linearVelocity;
            if(lv.magnitude != 0.0f)
            {
                lv.x = Mathf.Clamp(lv.x, -2.0f, 2.0f);
                lv.y = Mathf.Clamp(lv.y, -1.5f, 1.0f);
                
                rbOffset = Vector3.SmoothDamp(
                    rbOffset,
                    lv,
                    ref rbOffsetVelocity,
                    SmoothTime
                );
                targetPos += rbOffset;
            }
            else
            {
                rbOffset.y = Mathf.SmoothDamp(
                    rbOffset.y,
                    0.0f,
                    ref rbOffsetVelocity.y,
                    0.05f
                );
                targetPos += rbOffset;
            }
        }

        // Ignore isn't a good idea here since we have a good SmoothDamp
        // if(Vector3.Distance(targetPos, transform.position) <= IgnoreRangeRadius)
        // {
        //     transform.position += velocity * Time.deltaTime;
        //     velocity *= SlowDownRatio;
        //     return;
        // }

        transform.position = Vector3.SmoothDamp( // Unity's builtin function SmoothDamp do the calculations for us
            transform.position,
            targetPos,
            ref velocity,
            SmoothTime
        );
    }
}