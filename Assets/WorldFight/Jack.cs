using UnityEngine;
using UnityEngine.InputSystem;
//  Jack的Script
[RequireComponent(typeof(Rigidbody2D))]
public class Jack : MonoBehaviour{
    private Camera cam;

    private Transform rendererTrans;
    private Vector3 rendererTransScale;
    private Animator animator;
    private AnimationState animationState = AnimationState.LeftFront;
    public FacingDirection dir = FacingDirection.None;
    
    public GameObject HPBarPrefab;
    private HPBar HpBar;

    private Rigidbody2D rb;
    public float MoveSpeed;

    public float JumpSpeed;
    public float JumpBufferMaxTime;
    private float jumpBufferTimer = -0.1f;

    public float JumpHoldMaxTime;
    private float jumpHoldTimer = 0.0f;

    private bool isJumpReduced = false;
    private int groundTouchedCount = 0;
    private bool isGrounded => groundTouchedCount != 0;
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
            HpBar.MaxHP = 100.0f;
            HpBar.HP = 100.0f;
            HpBar.OnHpLE0 = () => {DeathManager.StartDeath(0.0f, 1.0f);};
            #if UNITY_EDITOR
            HpBar.name = string.Format("{0} - HPBar", this.name);
            #endif
        }

        rb = gameObject.GetComponent<Rigidbody2D>();

        foreach(Transform childTrans in transform)
        {
            GameObject child = childTrans.gameObject;
            if(child.name.Equals("Renderer"))
            {
                rendererTrans = childTrans;
                rendererTransScale = rendererTrans.localScale;

                if(!child.TryGetComponent<Animator>(out animator))
                {
                    Debug.LogError("Renderer child GameObject is expected to have a animator.");
                }
                animator.speed = 1.25f;
                animator.Play("Stand Left Front", 0, 0.0f);

                break;
            }
        }

        cam = Camera.main;
    }

    private bool leftPressed;
    private bool rightPressed;
    private bool jumpReleased;
    void Update(){
        // Do nothing if the game is stopped
        if(Time.timeScale == 0.0f){return;}
        
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

    float prevRunCycle = 0.0f;
    float prevFromtLeg = 1.0f;
    public void FixedUpdate()
    {
        if(Explode.Activated) {return;}

        float runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1.0f);
        float frontLeg = 
            (143.0f/166.0f <= runCycle || runCycle <= 15.0f/166.0f) || 
            (98.0f/166.0f <= runCycle && runCycle <= 140.0f/166.0f)
                ?1.0f
                :-1.0f; // 1 for left and -1 for right
        if (leftPressed){
            rb.linearVelocityX = -MoveSpeed;
            rendererTransScale.z = -Mathf.Abs(rendererTransScale.z);
            rendererTrans.localScale = rendererTransScale;
            dir = FacingDirection.Left;
            if(animationState != AnimationState.Walking)
            {
                animator.SetBool("walking", true);
                animator.SetBool("leftFront", false);
                animator.SetBool("rightFront", false);
                float normalizedFrameCount = 1.0f/166.0f * ((animationState == AnimationState.LeftFront)
                    ?101.0f
                    :57.0f
                    );
                animator.Play("Walking", 0, normalizedFrameCount);

                animationState = AnimationState.Walking;
            }
        }  
        else if (rightPressed) {
            rb.linearVelocityX = MoveSpeed;
            rendererTransScale.z = Mathf.Abs(rendererTransScale.z);
            rendererTrans.localScale = rendererTransScale;
            dir = FacingDirection.Right;
            if(animationState != AnimationState.Walking)
            {
                animator.SetBool("walking", true);
                animator.SetBool("leftFront", false);
                animator.SetBool("rightFront", false);
                float normalizedFrameCount = 1.0f/166.0f * ((animationState == AnimationState.LeftFront)
                    ?101.0f
                    :57.0f
                    );
                animator.Play("Walking", 0, normalizedFrameCount);

                animationState = AnimationState.Walking;
            }
        }
        else{
            rb.linearVelocityX = 0f;
            if(animationState == AnimationState.Walking)
            {
                if(prevFromtLeg != runCycle)
                {
                    animationState = (frontLeg==1.0f)
                        ?AnimationState.LeftFront
                        :AnimationState.RightFront;
                    animator.SetBool("walking", false);
                    animator.SetBool("leftFront", frontLeg==1.0f);
                    animator.SetBool("rightFront", frontLeg!=1.0f);
                }
            }
        }
        prevFromtLeg = frontLeg;
        prevRunCycle = runCycle;

        if(jumpReleased)
        {
            if(!isJumpReduced)
            {
                isJumpReduced = true;
            }
        }
        else if(!isJumpReduced && jumpHoldTimer <= JumpHoldMaxTime)
        {
            jumpHoldTimer += 0.02f; // FixedUpdate dt;
            rb.gravityScale = 0.0f;
        }
        else
        {
            rb.gravityScale = 1.7f;
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
            isJumpReduced = true;
            groundTouchedCount++;
        }
    }
    void OnCollisionExit2D(Collision2D other){
        // 在Jack離開其他實體時...

        GameObject colliderGameObject = other.collider.gameObject;

        if (colliderGameObject.layer == GlobalVariables.GroundLayer){
            groundTouchedCount--;
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        GameObject otherGameObject = other.gameObject;
        if(otherGameObject.layer == GlobalVariables.EnemyLayer)
        {
            HpBar.HP -= 0.1f;
        }
    }
}

enum AnimationState
{
    LeftFront,
    RightFront,
    Walking
}