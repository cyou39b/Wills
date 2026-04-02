using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//  Jack的Script
[RequireComponent(typeof(Rigidbody2D))]
public class Jack : MonoBehaviour//, IKnockbackable
{
    private Transform rendererTrans;
    private Vector3 rendererTransScale;
    private Animator animator;
    private AnimationState animationState = AnimationState.LeftFront;
    
    public GameObject HPBarPrefab;
    private HPBar HpBar;

    [NonSerialized] public Rigidbody2D rb;
    private bool _knockbackStun = false;
    private bool knocbackStun
    {
        get => _knockbackStun;
        set
        {
            if (value)
            {
                animator.speed = 1.0f;
            }
            else
            {
                animator.speed = 1.25f;
            }
            _knockbackStun = value;
        }
    }

    public static float MoveSpeed = 6.0f;

    public float JumpSpeed;
    public float JumpBufferMaxTime;
    private float jumpBufferTimer = -0.1f;

    public float JumpHoldMaxTime;
    private float jumpHoldTimer = 0.0f;

    private bool isJumpReduced = false;
    private bool isGrounded => leg.objectsStandingOn.Count != 0;
    
    private JacLeg leg;

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

        rendererTrans = transform.Find("Renderer");
        if(rendererTrans == null)
        {
            Debug.LogError("Jack should have a child gameobject that is the model");
        }
        rendererTransScale = rendererTrans.localScale;
        GameObject rendererChild = rendererTrans.gameObject;

        if(!rendererChild.TryGetComponent<Animator>(out animator))
        {
            Debug.LogError("Renderer child GameObject is expected to have a animator.");
        }
        animator.speed = 1.25f;
        animator.Play("Stand Left Front", 0, 0.0f);

        Transform legTrans = transform.Find("Jack Leg");
        if(legTrans == null)
        {
            Debug.LogError("Jack no leg trigger gameobject :(");
        }
        if(!legTrans.gameObject.TryGetComponent<JacLeg>(out leg))
        {
            Debug.LogError("Leg no JacLeg ?");
        }
    }

    private bool leftPressed;
    private bool rightPressed;
    private bool jumpReleased;
    private float prevRunCycle = 0.0f;
    private float prevFromtLeg = 1.0f;
    void Update(){
        // Do nothing if the game is stopped
        if(Time.timeScale == 0.0f){return;}
        
        leftPressed = Keyboard.current[GlobalVariables.Instance.MoveLeftKey].isPressed;
        rightPressed = Keyboard.current[GlobalVariables.Instance.MoveRightKey].isPressed;
        
        float runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1.0f);
        float frontLeg = 
            (143.0f/166.0f <= runCycle || runCycle <= 15.0f/166.0f) || 
            (98.0f/166.0f <= runCycle && runCycle <= 140.0f/166.0f)
                ?1.0f
                :-1.0f; // 1 for left and -1 for right
        if (leftPressed)
        {
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
        else if (rightPressed)
        {
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
        else
        {
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
        
        jumpBufferTimer -= Time.deltaTime;
        if (Keyboard.current[GlobalVariables.Instance.JumpKey].wasPressedThisFrame)
        {
            // Jump Buffering：把上次JumpKey被按下的時間記下來
            // 在碰到地板時檢查是否上次按下Jump的時間還算近
            jumpBufferTimer = JumpBufferMaxTime;
        }

        jumpReleased = Keyboard.current[GlobalVariables.Instance.JumpKey].wasReleasedThisFrame;
    }

    void FixedUpdate()
    {
        if(Explode.Activated) {return;}

        if (knocbackStun)
        {
            if(rb.linearVelocityX <= 0.1f && isGrounded) {knocbackStun = false;}
        }
        else
        {
            if (leftPressed){
                rb.linearVelocityX = -MoveSpeed;
                rendererTransScale.z = -0.19f;
                rendererTrans.localScale = rendererTransScale;
            }  
            else if (rightPressed) {
                rb.linearVelocityX = MoveSpeed;
                rendererTransScale.z = 0.19f;
                rendererTrans.localScale = rendererTransScale;
            }
            else{
                rb.linearVelocityX = 0.0f;
            }
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
            jumpHoldTimer += 0.02f; // FixedUpdate dt;
            rb.gravityScale = 0.0f;
        }
        else
        {
            rb.gravityScale = 1.7f;
        }

        // if(isGrounded && rb.linearVelocityY == 0.0f && jumpBufferTimer >= 0.0f){
        if(isGrounded && jumpBufferTimer >= 0.0f){
            Jump();
        }
    }

    public float JumpPushForceModifier;
    void Jump()
    {
        rb.linearVelocityY = JumpSpeed;
        jumpHoldTimer = 0.0f;
        isJumpReduced = false;
        jumpBufferTimer = -0.1f; // 把Timer設成負值，避免出現什麼奇怪的bug

        foreach(KeyValuePair<GameObject, IKnockbackable> kv in leg.objectsStandingOn)
        {
            if(kv.Value == null){continue;}
            kv.Value.GetKnockbacked(Vector2.down, JumpSpeed * JumpPushForceModifier * rb.mass / Time.fixedDeltaTime, false, leg.transform.position);
        }
    }

    public void GetDamaged(float damage)
    {
        HpBar.HP -= damage;
    }
}

enum AnimationState
{
    LeftFront,
    RightFront,
    Walking
}