using UnityEngine;
using UnityEngine.InputSystem;
//  Jack的Script
public class Jack : MonoBehaviour{
    public Rigidbody2D rb;
    public float MoveSpeed, SlowerMoveSpeed, JumpForce;
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
        // 根據mouse的position調整Jack的面向
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
    }

    public void FixedUpdate()
    {
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

        jumpBufferTimer -= Time.deltaTime;
        if (Keyboard.current[GlobalVariables.Instance.JumpKey].wasPressedThisFrame)
        {
            jumpBufferTimer = JumpBufferMaxTime;
            // Jump Buffering：把上次JumpKey被按下的時間記下來
            // 在碰到地板時檢查是否上次按下Jump的時間還算近
        }
        if(isGrounded && jumpBufferTimer >= 0.0f){
            rb.AddForceY(JumpForce);
            jumpBufferTimer = -0.1f; // 把Timer設成負值，避免出現什麼奇怪的bug
        }

    }

    void OnCollisionEnter2D(Collision2D other){
        // 在Jack碰到其他實體時...

        GameObject colliderGameObject = other.collider.gameObject;

        if (colliderGameObject.layer == GlobalVariables.GroundLayer)
        {
            // 如果是ground就把isGrounded 設成 true
            isGrounded = true;
        }
        // else if (other.collider.CompareTag("Wills"))
        // {
        // }
    }
    void OnCollisionExit2D(Collision2D other){
        // 在Jack離開其他實體時...

        GameObject colliderGameObject = other.collider.gameObject;

        if (colliderGameObject.layer == GlobalVariables.GroundLayer){
            // 如果是ground就把isGrounded 設成 false
            isGrounded = false;
        }
    }
    
}
