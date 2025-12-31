using UnityEngine;
using UnityEngine.InputSystem;

public class Jack : MonoBehaviour{
    public Rigidbody2D rb;
    public float MoveSpeed, SlowerMoveSpeed, JumpPower;
    public float JumpBufferMaxTime;
    private float jumpBufferTimer = -0.1f;
    private float facingDirection = -1.0f;
    bool isGrounded = false;
    public SpriteRenderer Sprerr;
    void Update(){
        // Do nothing if the game is stopped
        if(Time.timeScale == 0.0f){return;}

        Vector2 mousePixelPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mousePixelPosition);
        if(mousePosition.x > transform.position.x)
        {
            facingDirection = 1.0f;
            Sprerr.flipX = false;
        }
        else
        {
            facingDirection = -1.0f;
            Sprerr.flipX = true;
        }

        // Left/Right moving logic
        if (Keyboard.current[GlobalVariables.Instance.MoveLeftKey].isPressed){
            if(facingDirection == -1.0f)
            {
                rb.linearVelocityX = -MoveSpeed;
            }
            else
            {
                rb.linearVelocityX = -SlowerMoveSpeed;
            }
        }  
        else if (Keyboard.current[GlobalVariables.Instance.MoveRightKey].isPressed){
            if(facingDirection == 1.0f)
            {
                rb.linearVelocityX = MoveSpeed;
            }
            else
            {
                rb.linearVelocityX = SlowerMoveSpeed;
            }
        }
        else{
            rb.linearVelocityX = 0f;
        }

        // Jump logic with jump buffering
        jumpBufferTimer -= Time.deltaTime;
        if (Keyboard.current[GlobalVariables.Instance.JumpKey].wasPressedThisFrame)
        {
            jumpBufferTimer = JumpBufferMaxTime;
        }
        if(isGrounded && jumpBufferTimer >= 0.0f){
            rb.linearVelocityY = JumpPower;
            jumpBufferTimer = -0.1f;
        }

    }

    // It turns out that there's a function that detect collisions
    // And it's better on dealing with things like floors or walls
    void OnCollisionEnter2D(Collision2D other){
        if (other.collider.CompareTag("Ground")){
            isGrounded = true;
        }
        else if (other.collider.CompareTag("Wills"))
        {
        }
    }
    void OnCollisionExit2D(Collision2D other){
        if (other.collider.CompareTag("Ground")){
            isGrounded = false;
        }
    }
    
}
