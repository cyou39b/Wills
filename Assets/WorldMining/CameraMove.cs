using UnityEngine;

public class CameraMove : MonoBehaviour{
    public Transform Target;
    public Vector3 Offset;
    public float SmoothTime;
    public float IgnoreRangeRadius;
    public float SlowDownRatio;

    private Vector3 velocity = Vector3.zero;
    void Start()
    {
        transform.position = Target.position + Offset;
    }

    void FixedUpdate()
    {
        Vector3 targetPos = Target.position + Offset;
        
        if(Vector3.Distance(targetPos, transform.position) <= IgnoreRangeRadius)
        {
            transform.position += velocity * Time.deltaTime;
            velocity *= SlowDownRatio;
            return;
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            SmoothTime
        );
    }
}