using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//  Jack的Script
[RequireComponent(typeof(Rigidbody2D), typeof(AudioSource))]
public class Jack : MonoBehaviour, IHaveHP
{
    private Transform rendererTrans;
    private Vector3 rendererTransScale;
    private Animator animator;
    private AnimationState animationState = AnimationState.LeftFront;
    
    public GameObject HPBarPrefab;
    [NonSerialized] public HPBar HpBar;
    HPBar IHaveHP.HPBar => HpBar;

    public Rigidbody2D rb; // for other script to use before Jack's Start

    public static float MoveSpeed = 6.0f;

    public float JumpSpeed;
    public float JumpBufferMaxTime;
    private float jumpBufferTimer = -0.1f;

    public float JumpHoldMaxTime;
    private float jumpHoldTimer = 0.0f;

    private bool isJumpReduced = false;
    private bool isGrounded => leg.objectsStandingOn.Count != 0;

    private Gun weapon;
    private ThrowPotion throwPotion;
    private JacLeg leg;

    private AudioSource audioSource;
    public AudioClip DeathSoundEffect;

    public GameObject HeadStomePrefab;

    public static bool inEntrence = true;

    public void Start()
    {
        inEntrence = true;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = DeathSoundEffect;

        GameObject newObj = Instantiate(
            HPBarPrefab,
            transform.position,
            transform.rotation
        );
        
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
    
        Transform weaponChildTrans = transform.Find("Weapon");
        if(weaponChildTrans == null)
        {
            Debug.LogError("Jack don't have weapon?");
        }
        if(!weaponChildTrans.gameObject.TryGetComponent<Gun>(out weapon))
        {
            Debug.LogError("Weapon gameobject missing component");
        }

        Transform throwPotionChildTrans = transform.Find("throw");
        if(throwPotionChildTrans == null)
        {
            Debug.LogError("throw child?");
        }
        if(!throwPotionChildTrans.TryGetComponent<ThrowPotion>(out throwPotion))
        {
            Debug.LogError("Missing component");
        }
        throwPotion.gameObject.SetActive(false);

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
            HpBar.OnHpLE0 = () => {
                Transform trans = transform;
                Instantiate(HeadStomePrefab, trans.position, trans.rotation);
                audioSource.Play();
                rendererChild.SetActive(false);
                DeathManager.StartDeath(0.0f, 1.0f);
            };
            #if UNITY_EDITOR
            HpBar.name = string.Format("{0} - HPBar", this.name);
            #endif
        }
    }

    public void StartPlayerAction()
    {
        if(!ThrowPotion.inThrow)
        {
            weapon.enabled = true;
            weapon.gameObject.SetActive(true);
        }
        else
        {
            throwPotion.gameObject.SetActive(true);
        }
        inEntrence = false;
    }
    public void EndPlayerAction()
    {
        inEntrence = true;
        weapon.enabled = false;
        weapon.gameObject.SetActive(false);
    }
    public void DisableGun()
    {
        weapon.enabled = false;
        weapon.gameObject.SetActive(false);
    }
    public void EnableGun()
    {
        weapon.enabled = true;
        weapon.gameObject.SetActive(true);

    }

    public void StartThrowPotion(float effectAmount)
    {
        if(!inEntrence) {throwPotion.gameObject.SetActive(true);}
        throwPotion.effectAmount = effectAmount;
        throwPotion.throwCallback = () =>
        {
            EnableGun();
            throwPotion.gameObject.SetActive(false);
            ThrowPotion.inThrow = false;
        };
        DisableGun();
        ThrowPotion.inThrow = true;
    }


    private bool leftPressed;
    private bool rightPressed;
    private bool jumpReleased;
    private float prevRunCycle = 0.0f;
    private float prevFromtLeg = 1.0f;
    void Update(){
        // Do nothing if the game is stopped
        if(Time.timeScale == 0.0f){return;}
        
        float runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1.0f);
        float frontLeg = 
            (143.0f/166.0f <= runCycle || runCycle <= 15.0f/166.0f) || 
            (98.0f/166.0f <= runCycle && runCycle <= 140.0f/166.0f)
                ?1.0f
                :-1.0f; // 1 for left and -1 for right

        if(BackpackLogic.IsBackpackOpening) 
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
            return;
        }
        
        leftPressed = Keyboard.current[GlobalVariables.Instance.MoveLeftKey].isPressed;
        rightPressed = Keyboard.current[GlobalVariables.Instance.MoveRightKey].isPressed;

        if (leftPressed && weapon.attackFreezeTimer<0.0f)
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
        else if (rightPressed && weapon.attackFreezeTimer<0.0f)
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

    private bool prevFrameGrounded = false;
    void FixedUpdate()
    {
        if(Explode.Activated) {return;}

        if(BackpackLogic.IsBackpackOpening)
        {
            rb.linearVelocityX = 0.0f;

            isJumpReduced = true;
            jumpHoldTimer = JumpHoldMaxTime;
            rb.gravityScale = 1.0f;
            return;
        }

        if(weapon.attackFreezeTimer >= 0.0f)
        {
            rb.linearVelocityX = 0.0f;
            if(weapon.facingLeft)
            {
                rendererTransScale.z = -0.19f;
                rendererTrans.localScale = rendererTransScale;
            }
            else
            {
                rendererTransScale.z = 0.19f;
                rendererTrans.localScale = rendererTransScale;
            }
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

            if(jumpReleased)
            {
                isJumpReduced = true;
            }

            if(!isJumpReduced && jumpHoldTimer <= JumpHoldMaxTime)
            {
                jumpHoldTimer += Time.fixedDeltaTime;
            }
            else
            {
                rb.gravityScale = 1.0f;
            }

            if(prevFrameGrounded && jumpBufferTimer >= 0.0f){
                Jump();
            }
            prevFrameGrounded = isGrounded;
        }
    }

    public float JumpPushForceModifier;
    void Jump()
    {
        rb.linearVelocityY = JumpSpeed;
        rb.gravityScale = 0.0f;
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