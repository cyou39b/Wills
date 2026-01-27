using UnityEngine;
using UnityEngine.InputSystem;
//  Jack的Script
public class Jack : MonoBehaviour{
    public SpriteRenderer Sprerr;
    private float facingDirection = -1.0f;
    
    public GameObject HPBarPrefab;
    private HPBar HpBar;

    private Rigidbody2D rb;
    private BoxCollider2D boxColl;
    public float MoveSpeed, SlowerMoveSpeed;

    public float JumpSpeed;
    public float JumpBufferMaxTime;
    private float jumpBufferTimer = -0.1f;

    public float JumpHoldMaxTime;
    private float jumpHoldTimer = 0.0f;

    private bool isJumpReduced = false;  
    private bool isGrounded = false; 
    // FIXME?: This method could cause some problem if Jack \
    // can touch multiple grounds at once. 
    
    public void Start()
    {
        GameObject newObj = Instantiate(
            HPBarPrefab,
            transform.position,
            transform.rotation
        );
        
        if(!newObj.TryGetComponent<HPBar>(out this.HpBar))
        {
            Debug.LogErrorFormat("Failed to get HPBar component from gameObject created for GameObject named {}.", this.name);
        }
        else
        {
            HpBar.Followee = gameObject;
            HpBar.Offset = new Vector3(0f, 1.5f, 0f);
            HpBar.SmoothTime = 0.05f;
            HpBar.SetMaxHP(100.0f);
            HpBar.SetHP(100.0f);
            #if UNITY_EDITOR
            HpBar.name = string.Format("{0} - HPBar", this.name);
            #endif
        }

        rb = gameObject.GetComponent<Rigidbody2D>();
        boxColl = rb.GetComponent<BoxCollider2D>();

    }

    private Vector3 mousePos;
    private bool leftPressed;
    private bool rightPressed;
    private bool jumpReleased;
    void Update(){
        // Do nothing if the game is stopped
        if(Time.timeScale == 0.0f){return;}
        
        Vector2 mousePixelPosition = Mouse.current.position.ReadValue();
        mousePos = Camera.main.ScreenToWorldPoint(mousePixelPosition);
        
        leftPressed = Keyboard.current[GlobalVariables.Instance.MoveLeftKey].isPressed;
        rightPressed = Keyboard.current[GlobalVariables.Instance.MoveRightKey].isPressed;
        
        jumpBufferTimer -= Time.deltaTime;
        if (Keyboard.current[GlobalVariables.Instance.JumpKey].wasPressedThisFrame)
        {
            // Jump Buffering：把上次JumpKey被按下的時間記下來
            // 在碰到地板時檢查是否上次按下Jump的時間還算近
            jumpBufferTimer = JumpBufferMaxTime;
        }

        jumpReleased = Keyboard.current[GlobalVariables.Instance.JumpKey].wasReleasedThisFrame;
    }

    public void FixedUpdate()
    {
        if(Explode.Activated) {return;}
        // 根據mouse的position調整Jack的面向
        if(mousePos.x > transform.position.x)
        {
            facingDirection = 1.0f;
            Sprerr.flipX = false;
        }
        else
        {
            facingDirection = -1.0f;
            Sprerr.flipX = true;
        }

        if (leftPressed){
            if(facingDirection > 0.0f)
            {
                rb.linearVelocityX = -MoveSpeed;
            }
            else
            {
                rb.linearVelocityX = -SlowerMoveSpeed;
            }
        }  
        else if (rightPressed) {
            if(facingDirection > 0.0f)
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

        if(jumpReleased)
        {
            if(!isJumpReduced)
            {
                isJumpReduced = true;
            }
        }
        else if(!isJumpReduced && jumpHoldTimer <= JumpHoldMaxTime)
        {
            jumpHoldTimer += Time.deltaTime;
            rb.gravityScale = 0.0f;
        }
        else
        {
            rb.gravityScale = 1.5f;
        }

        if(isGrounded && rb.linearVelocityY == 0.0f && jumpBufferTimer >= 0.0f){
            rb.linearVelocityY = JumpSpeed;
            jumpHoldTimer = 0.0f;
            isJumpReduced = false;
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
            isJumpReduced = true;
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

    // void OnTriggerStay2D(Collider2D collision)
    // {
    // }
}
