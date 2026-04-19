using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HeadStone : MonoBehaviour
{
    // private SpriteRenderer spriteRenderer;

    // private int frame = 0;
    // private Vector3 initPos;

    // public int endFrame;
    // public float riseSpeed;
    // public float cycleSpeed;
    // public float cycleStrength;
    // void Start(){initPos = transform.position; spriteRenderer = GetComponent<SpriteRenderer>();}

    // void Update()
    // {
    //     Color color = spriteRenderer.color;
    //     color.a = 1.0f - (float)frame/endFrame;
    //     spriteRenderer.color = color;
    // }

    // void FixedUpdate()
    // {
    //     frame++;
    //     if(frame == endFrame) {Destroy(gameObject);}
    //     transform.position = initPos + new Vector3(cycleStrength * Mathf.Sin(frame * cycleSpeed), frame*riseSpeed, 0.0f);
    // }
}
