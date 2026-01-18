using UnityEngine;

// 讓Camera的移動變smooth

public class CameraMove : MonoBehaviour{
    public Transform Target;
    public Vector3 Offset = new Vector3(0.0f, 0.0f, -10.0f);
    public float SmoothTime;
    
    public float IgnoreRangeRadius;
    // 如果camera和target的距離小於這個value的話就不需要再移動
    // 可以避免玩家再進行很小的移動時camera一直走走停停的

    public float SlowDownRatio;

    private Vector3 velocity = Vector3.zero;
    void Start()
    {
        transform.position = Target.position + Offset;
    }

    void Update()
    {
        Vector3 targetPos = Target.position + Offset;
        
        if(Vector3.Distance(targetPos, transform.position) <= IgnoreRangeRadius)
        {
            transform.position += velocity * Time.deltaTime;
            velocity *= SlowDownRatio;
            return;
        }

        transform.position = Vector3.SmoothDamp( // Unity's builtin function SmoothDamp do the calculations for us
            transform.position,
            targetPos,
            ref velocity,
            SmoothTime
        );
    }
}