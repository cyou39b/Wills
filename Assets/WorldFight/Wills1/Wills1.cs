using UnityEngine;
using UnityEngine.InputSystem;

public class Wills : MonoBehaviour
{
    public Animator Anmor;
    // private bool walking = false;
    // private static readonly string anmorIsWalkingTag="isWalking";
    public Rigidbody2D Rgd;
    public Vector2 PushForce;

    public SpriteRenderer SpRerr;
    public int HP;
    void Update()
    {
        if(transform.position.x >= 0)
        {
            Rgd.linearVelocityX -= 10 * Time.deltaTime;
        }
        else
        {
            Rgd.linearVelocityX += 10* Time.deltaTime;
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            HP -= 10;
            if(HP <= 0)
            {
                Destroy(gameObject);
                return;
            }

            Rgd.AddForce(PushForce);
        }
    }
}
