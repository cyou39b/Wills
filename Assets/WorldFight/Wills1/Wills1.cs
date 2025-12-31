using UnityEngine;
using UnityEngine.InputSystem;

public class Wills : MonoBehaviour
{
    public Animator Anmor;
    // private bool walking = false;
    // private static readonly string anmorIsWalkingTag="isWalking";
    public Rigidbody2D Rgd;
    public float MoveSpeeed;
    public SpriteRenderer SpRerr;
    void Update()
    {
        Rgd.linearVelocityX = MoveSpeeed;
        if(transform.position.x >= 12.0f)
        {
            transform.position = transform.position + Vector3.left * 24;
        }
        // if (Keyboard.current.aKey.isPressed)
        // {
        //     Rgd.linearVelocityX = -MoveSpeeed;
        //     SpRerr.flipX = true;
        //     if (!walking)
        //     {
        //         walking = true;
        //         Anmor.SetBool(anmorIsWalkingTag, true);
        //     }
        // }
        // else if (Keyboard.current.dKey.isPressed)
        // {
        //     Rgd.linearVelocityX = MoveSpeeed;
        //     Sprerr.flipX = false;
        //     if (!walking)
        //     {
        //         walking = true;
        //         Anmor.SetBool(anmorIsWalkingTag, true);
        //     }
        // }
        // else
        // {
        //     if (walking)
        //     {
        //         Rgd.linearVelocityX = 0.0f;
        //         walking = false;
        //         Anmor.SetBool(anmorIsWalkingTag, false);
        //     }
        // }
    }
}
