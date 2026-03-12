using UnityEngine;

// 讓Camera的移動變smooth

public class CameraMove : MonoBehaviour{
    public Transform Target;
    public Vector3 Offset = new Vector3(0.0f, 0.0f, -10.0f);
    public float SmoothTime;
    
    private Vector3 velocity = Vector3.zero;
    void Start()
    {
        transform.position = Target.position + Offset;
    }

    void FixedUpdate()
    {
        Vector3 targetPos = Target.position + Offset;

        transform.position = Vector3.SmoothDamp( // Unity's builtin function SmoothDamp do the calculations for us
            transform.position,
            targetPos,
            ref velocity,
            SmoothTime
        );
    }
}