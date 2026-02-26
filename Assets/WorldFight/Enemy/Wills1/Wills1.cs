using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Wills : AbstractEnemy
{
    public Color[] BodyColors;
    public Color[] EyeColors;

    private const string anmorIsWalkingTag = "isWalking";
    private bool _walking = false;
    private bool walking 
    {
        get => _walking;
        set
        {
            if(_walking != value)
            {
                // Anmor.SetBool(AnmorIsWalkingTag, true); <- 把wills1的動畫調成正在移動
                // Anmor.SetBool(AnmorIsWalkingTag, false); <- 把動畫調成站著不動
                Anmor.SetBool(anmorIsWalkingTag, value);
                _walking = value;
            }
        }
    } 
    
    private Dictionary<string, DynamicAIAction> listeningJumpBlock = new Dictionary<string, DynamicAIAction>();

    private NavMeshAgent agent;

    public GameObject FireworkPrefab;

    protected override void Start()
    {
        base.Start();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.autoTraverseOffMeshLink = false;

        if(!agent.isOnNavMesh)
        {
            NavMeshQueryFilter filter = new NavMeshQueryFilter()
            {
                agentTypeID = agent.agentTypeID,
                areaMask = NavMesh.AllAreas
            };

            NavMeshHit info;
            if(NavMesh.SamplePosition(transform.position, out info, 10.0f, filter))
            {
                agent.Warp(info.position);
            }
            else
            {
                Debug.LogError("Agent Sample position failed.");
            }
        }

        if(BodyColors.Length != EyeColors.Length)
        {
            Debug.LogError("BodyColors.Length != EyeColors.Length");
        }
        if(BodyColors.Length == 0)
        {
            Debug.LogError("BodyColors.Length == 0");
        }

        int idx = Random.Range(0, BodyColors.Length);
        Mat.SetColor("_BodyColor", BodyColors[idx]);
        mainColor = BodyColors[idx];
        Mat.SetColor("_EyeColor", EyeColors[idx]);
    }

    protected override (float, float, Vector3?) HpBarData() 
        => (40.0f, 40.0f, new Vector3(0.0f, 1.4f, 0.0f));

    public void Update()
    {
        if(!isIntentPassive)
        {
            // not passive & agent not enabled => in attack/dodge intent and
            // direction is handled in intent coroutine
            if(agent.enabled)
            {
                Vector3 targetDirection = agent.steeringTarget - transform.position;
                if(targetDirection.x != 0.0f)
                {
                    SpRr.flipX = targetDirection.x < 0.0f;
                }
                walking = agent.desiredVelocity.magnitude >= 0.0f;
            }
        }
        else
        {
            SpRr.flipX = rb.linearVelocity.x >= 0.0f;
            walking = true;
        }
    }

    protected override void MainProcessIntent()
    {
        restart:
        switch(intent)
        {
            case Intent.Idle:
                SetIntent(Intent.ChasePlayer, false);
                goto case Intent.ChasePlayer;
            case Intent.ChasePlayer:
                ProcessChasePlayerIntent();
                break;
            case Intent.Jump:
                ProcessJumpIntent();
                break;
            case Intent.WalkOffEdge:
                ProcessWalkOffEdgeIntent();
                break;
            case Intent.WaitUntilStill:
                ProcessWaitUntilStillIntent();
                if(intent != Intent.WaitUntilStill)
                {
                    goto restart;
                }
                break;
            case Intent.WaitUntilGround:
                ProcessWaitUntilGroundedIntent();
                if(intent != Intent.WaitUntilGround)
                {
                    goto restart;
                }
                break;
            case Intent.GetKnockbacked:
                ProcessGetKnockbackedIntent();
                break;
            default:
                Debug.LogError($"{nameof(Wills)} doesn't implement case {intent} for MainProccesIntent");
                break;
        }
    }

    protected override Intent intent
    { 
        // haha fucking set function
        get => base.intent; 
        set
        {
            switch(value)
            {
                case Intent.ChasePlayer:
                    chasePlayer_counter = chasePlayer_count - 1;
                    break;
                case Intent.Jump:
                    jumpIntent_stage = 0;
                    break;
                case Intent.WalkOffEdge:
                    walkOffEdgeIntent_stage = 0;
                    break;
                case Intent.WaitUntilGround:
                    waitUntilGround_stage = 0;
                    break;
            }
            base.intent = value;
        } 
    }

    public override void GetKnockbacked(Vector2 direction, float power, bool stun)
    {
        if(intent == Intent.GetKnockbacked)
        {
            getKnockbackedIntent_force += direction * power;
            getKnockbackedIntent_stun |= stun;
        }
        else
        {
            getKnockbackedIntent_force = direction * power;
            getKnockbackedIntent_stun = stun;
        }
        SetIntent(Intent.GetKnockbacked, true);
    }

    public float xEpsilon, yEpsilon;
    private Vector2 getKnockbackedIntent_force;
    private bool getKnockbackedIntent_stun;
    private void ProcessGetKnockbackedIntent()
    {
        agent.enabled = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1.0f;
        rb.AddForce(getKnockbackedIntent_force);
        SetIntent(Intent.WaitUntilStill, false, Intent.GetKnockbacked);
        
        getKnockbackedIntent_force = Vector2.zero;
        getKnockbackedIntent_stun = false;
    }

    private void ProcessWaitUntilStillIntent()
    {
        if(Mathf.Abs(rb.linearVelocityX) <= xEpsilon && Mathf.Abs(rb.linearVelocityY) <= yEpsilon)
        {
            SetIntent(Intent.WaitUntilGround, false, Intent.WaitUntilStill);
        }
    }

    private int waitUntilGround_stage = 0;
    private void ProcessWaitUntilGroundedIntent()
    {
        if(waitUntilGround_stage == 0)
        {
            if(rb.linearVelocityY == 0.0f) 
            {
                waitUntilGround_stage = 3;
            }
            else
            {
                waitUntilGround_stage = 1;
            }
        }

        if(waitUntilGround_stage == 1)
        {
            // wait until it's falling
            if(rb.linearVelocityY < 0.0f) {waitUntilGround_stage = 2;}
        }

        if(waitUntilGround_stage == 2)
        {
            // wait while it's still falling
            if(rb.linearVelocityY == 0.0f) {waitUntilGround_stage = 3;}
        }

        if(waitUntilGround_stage == 3)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;

            NavMeshQueryFilter filter = new NavMeshQueryFilter()
            {
                agentTypeID = agent.agentTypeID,
                areaMask = NavMesh.AllAreas
            };
            NavMeshHit hit;
            if(NavMesh.SamplePosition(transform.position, out hit, 10.0f, filter)) // WARN: Magic number
            {
                agent.Warp(hit.position);
            }
            else
            {
                Debug.LogError($"Agent Can't Warp, at {transform.position}");
            }
            agent.enabled = true;
            SetIntent(Intent.Idle, false, Intent.WaitUntilGround);
        }
    }

    private const int chasePlayer_count = 8;
    private int chasePlayer_counter = 0;
    private void ProcessChasePlayerIntent()
    {
        if(++chasePlayer_counter != chasePlayer_count){return;}
        else{chasePlayer_counter = 0;}

        agent.SetDestination(playerTrans.position);

        DynamicAIAction action = null;
        if(
            agent.isOnOffMeshLink && 
            listeningJumpBlock.ContainsKey(agent.currentOffMeshLinkData.owner.name)
        ) {
            action = listeningJumpBlock[agent.currentOffMeshLinkData.owner.name];
        }
        else if(
            agent.nextOffMeshLinkData.owner != null &&
            listeningJumpBlock.ContainsKey(agent.nextOffMeshLinkData.owner.name)
        ) {
            action = listeningJumpBlock[agent.nextOffMeshLinkData.owner.name];
        }
        if(action != null)
        {
            switch (action.Type)
            {
                case DynamicAIAction.ActionType.Jump:
                    jumpIntent_action = action;
                    SetIntent(Intent.Jump);
                    break;
                case DynamicAIAction.ActionType.WalkOffEdge:
                    walkOffEdgeIntent_action = action;
                    SetIntent(Intent.WalkOffEdge);
                    break;
            }
        }
    }

    [ContextMenu("Log stuffs")]
    public void LogStuffs()
    {
        string listners = string.Join(Environment.NewLine, listeningJumpBlock);
        Debug.Log($"name: {name}, intent: {intent}, listeners: {listners}, onlink: {agent.isOnOffMeshLink}, {jumpIntent_stage}, {jumpIntent_waitFrameCounter}, {jumpIntent_waitFrameTarget}");
    }

    private DynamicAIAction jumpIntent_action;
    private int jumpIntent_waitFrameCounter = 0;
    private int jumpIntent_waitFrameTarget;
    private int jumpIntent_stage = 0;
    private void ProcessJumpIntent()
    {
        if(jumpIntent_stage == 0)
        {
            Vector2 agentVelocity = agent.velocity;
            agent.enabled = false;

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = agentVelocity;
            rb.gravityScale = 1.0f;

            jumpIntent_waitFrameTarget = Random.Range(0, jumpIntent_action.TimeRange);
            jumpIntent_stage = 1;
        }
        else
        {
            if(jumpIntent_waitFrameCounter++ >= jumpIntent_waitFrameTarget)
            {
                jumpIntent_waitFrameCounter = 0;
                rb.linearVelocity = new Vector2(
                    jumpIntent_action.XSpeed,
                    jumpIntent_action.JumpSpeed + Random.Range(-jumpIntent_action.JumpSpeedRange, jumpIntent_action.JumpSpeedRange)
                );
                SetIntent(Intent.WaitUntilGround);
            }
        }
    }

    private DynamicAIAction walkOffEdgeIntent_action;
    private int walkOffEdgeIntent_stage = 0;
    private void ProcessWalkOffEdgeIntent()
    {
        if(walkOffEdgeIntent_stage == 0)
        {
            agent.enabled = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1.0f;
            rb.linearVelocity = new Vector2(walkOffEdgeIntent_action.XSpeed, 0.0f);
            SpRr.flipX = walkOffEdgeIntent_action.XSpeed <= 0.0f;
            walking = true;
            walkOffEdgeIntent_stage = 1;
        }
        else
        {
            rb.linearVelocityX = walkOffEdgeIntent_action.XSpeed;
            if(
                (walkOffEdgeIntent_action.XSpeed < 0.0f)
                    ?transform.position.x < walkOffEdgeIntent_action.TargetX
                    :transform.position.x > walkOffEdgeIntent_action.TargetX
            )
            {
                SetIntent(Intent.WaitUntilGround);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        GameObject otherGameObject = other.gameObject;

        DynamicAIAction jumpBlock;
        if(otherGameObject.TryGetComponent<DynamicAIAction>(out jumpBlock))
        {
            // Debug.Log($"Add listener {jumpBlock.Link.name}");
            listeningJumpBlock.Add(jumpBlock.Link.name, jumpBlock);
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Field"))
        {
            OnOutOfField();
            return;
        }

        GameObject otherGameObject = other.gameObject;

        DynamicAIAction jumpBlock;
        if(otherGameObject.TryGetComponent<DynamicAIAction>(out jumpBlock))
        {
            // Debug.Log($"Remove listener {jumpBlock.Link.name}");
            listeningJumpBlock.Remove(jumpBlock.Link.name);
        }
    }
}
