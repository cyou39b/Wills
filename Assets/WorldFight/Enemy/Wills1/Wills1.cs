using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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

    private Dictionary<string, DynamicAIAction> listeningDynamicAction = new Dictionary<string, DynamicAIAction>();

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

    protected override (float, float, Vector3) HpBarData 
        => (40.0f, 40.0f, new Vector3(0.0f, 2.0f, 0.0f));

    public void Update()
    {
        walking = (agent.enabled)?agent.velocity.magnitude>=0.0f:rb.linearVelocity.magnitude>=0.0f;

        Vector3 dir = (agent.enabled) 
            ?agent.steeringTarget-transform.position
            :rb.linearVelocity;
        // Debug.Log(dir.ToString());
        switch(base.FacingDirection)
        {
            case AIFacingDirection.SameAsMoving:
                SpRr.flipX = dir.x<=0.0f;
                break;
            case AIFacingDirection.NegFromMoving:
                SpRr.flipX = dir.x>=0.0f;
                break;
            case AIFacingDirection.None:
                //* Nothin
                break;
        }

        // if(!isIntentPassive)
        // {
        //     // not passive & agent not enabled => in attack/dodge intent and
        //     // direction is handled in intent coroutine
        //     if(agent.enabled)
        //     {
        //         Vector3 targetDirection = agent.steeringTarget - transform.position;
        //         if(targetDirection.x != 0.0f)
        //         {
        //             SpRr.flipX = targetDirection.x < 0.0f;
        //         }
        //         walking = agent.desiredVelocity.magnitude >= 0.0f;
        //     }
        // }
        // else
        // {
        //     SpRr.flipX = rb.linearVelocity.x >= 0.0f;
        //     walking = true;
        // }
    }

    protected override void MainProcessIntent()
    {
        restart:
        switch(intent)
        {
            case Intent.Idle:
                if (!agent.enabled)
                {
                    SafeWarp();
                    agent.enabled = true;
                }
                SetIntent(Intent.ChasePlayer, AIFacingDirection.SameAsMoving, false);
                goto case Intent.ChasePlayer;
            case Intent.ChasePlayer:
                ProcessChasePlayerIntent();
                break;
            case Intent.Attack:
                ProcessAttackIntent();
                break;
            case Intent.PrepJump:
                ProcessPrepJumpIntent();
                if(intent == Intent.Jump) {goto case Intent.Jump;}
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

    public override Intent intent
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
                case Intent.PrepJump:
                    prepJumpIntent_stage = 0;
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
        getKnockbackedIntent_force += direction * power;
        getKnockbackedIntent_stun |= stun;
        SetIntent(Intent.GetKnockbacked, AIFacingDirection.NegFromMoving, true);
    }

    public float xEpsilon, yEpsilon;
    private Vector2 getKnockbackedIntent_force = Vector2.zero;
    private bool getKnockbackedIntent_stun = false;
    private void ProcessGetKnockbackedIntent()
    {
        if(knockbackablesInContact.Count > 0)
        {
            Vector2 normalizeForce = getKnockbackedIntent_force.normalized;
            float power = getKnockbackedIntent_force.magnitude;

            float totalSimilarity = 0.0f;
            foreach(IKnockbackable knockbackable in knockbackablesInContact)
            {
                Vector2 dPos = knockbackable.gameObject.transform.position - transform.position;
                float thisSimilarity  = Vector2.Dot(dPos.normalized, normalizeForce); // Since direction's magnitude SHOULD be 1
                if(thisSimilarity > 0.0f) {totalSimilarity += thisSimilarity;}
            }

            foreach(IKnockbackable knockbackable in knockbackablesInContact)
            {
                Vector2 dPos = knockbackable.gameObject.transform.position - transform.position;
                float thisSimilarity  = Vector2.Dot(dPos.normalized, normalizeForce); // Since direction's magnitude SHOULD be 1
                if(thisSimilarity > 0.0f)
                {
                    knockbackable.GetKnockbacked(dPos.normalized, thisSimilarity * power * collisionForceGivePercentage, false);
                }
            }

            getKnockbackedIntent_force *= Mathf.Max(0.0f, 1.0f - totalSimilarity * (1.0f-collisionForceKeepPercentage));
        }

        Debug.Log($"{name} got knockbacked with force: {getKnockbackedIntent_force}");

        agent.enabled = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1.0f;
        rb.AddForce(getKnockbackedIntent_force);
        SetIntent(Intent.WaitUntilStill, AIFacingDirection.NegFromMoving ,  false, Intent.GetKnockbacked);
        
        getKnockbackedIntent_force = Vector2.zero;
        getKnockbackedIntent_stun = false;
    }

    private void ProcessWaitUntilStillIntent()
    {
        if(Mathf.Abs(rb.linearVelocityX) <= xEpsilon && Mathf.Abs(rb.linearVelocityY) <= yEpsilon)
        {
            SetIntent(Intent.WaitUntilGround, AIFacingDirection.None, false, Intent.WaitUntilStill);
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

            SafeWarp();
            agent.enabled = true;
            SetIntent(Intent.Idle, AIFacingDirection.None, false, Intent.WaitUntilGround); // FIXME?: fail to set intent sometimes
        }
    }

    private const int chasePlayer_count = 8;
    private int chasePlayer_counter = 0;
    private void ProcessChasePlayerIntent()
    {
        if(++chasePlayer_counter != chasePlayer_count){return;}
        else{chasePlayer_counter = 0;}

        RaycastHit2D info = Physics2D.Raycast(playerTrans.position, Vector2.down, 100.0f, GlobalVariables.GroundLayerMask);
        agent.SetDestination(info.point);

        DynamicAIAction action = null;
        if(
            agent.isOnOffMeshLink && 
            listeningDynamicAction.ContainsKey(agent.currentOffMeshLinkData.owner.name)
        ) {
            action = listeningDynamicAction[agent.currentOffMeshLinkData.owner.name];
        }
        else if(
            agent.nextOffMeshLinkData.owner != null &&
            listeningDynamicAction.ContainsKey(agent.nextOffMeshLinkData.owner.name)
        ) {
            action = listeningDynamicAction[agent.nextOffMeshLinkData.owner.name];
        }
        if(action != null)
        {
            switch (action.Type)
            {
                case DynamicAIAction.ActionType.Jump:
                    prepJumpIntent_pendingAction = action;
                    SetIntent(Intent.PrepJump, AIFacingDirection.SameAsMoving);
                    break;
                case DynamicAIAction.ActionType.WalkOffEdge:
                    walkOffEdgeIntent_action = action;
                    SetIntent(Intent.WalkOffEdge, AIFacingDirection.SameAsMoving);
                    break;
            }
        }
    }

    [ContextMenu("Log stuffs")]
    public void LogStuffs()
    {
        string listners = string.Join(Environment.NewLine, listeningDynamicAction);
        string agentVelocity = agent.enabled?agent.velocity.ToString():"Not enabled";
        Debug.Log($"name: {name}, intent: {intent}, listeners: {listners}, onlink: {agent.isOnOffMeshLink}, agentVelocity: {agentVelocity}, linear velocity: {rb.linearVelocity}");
    }

    private void ProcessAttackIntent()
    {
    }

    private DynamicAIAction prepJumpIntent_pendingAction = null;
    private Vector2 prepJumpIntent_pendingVelocity;
    private int prepJumpIntent_stage = 0;
    private void ProcessPrepJumpIntent()
    {
        if(prepJumpIntent_stage == 0)
        {
            prepJumpIntent_pendingVelocity = new Vector2(Mathf.Sign(prepJumpIntent_pendingAction.gameObject.transform.position.x-transform.position.x)*agent.speed, 0.0f);
            agent.enabled = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1.0f;
            rb.linearVelocity = prepJumpIntent_pendingVelocity;

            prepJumpIntent_stage = 1;
        }
        else
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(prepJumpIntent_pendingAction.gameObject.transform.position.x-transform.position.x)*agent.speed, 0.0f);
            if(prepJumpIntent_pendingAction.JumpRangeLeft <= transform.position.x &&
               transform.position.x <= prepJumpIntent_pendingAction.JumpRangeRight
            ){
                jumpIntent_action = prepJumpIntent_pendingAction;
                SetIntent(Intent.Jump, AIFacingDirection.SameAsMoving);
            }
        }
    }

    private DynamicAIAction jumpIntent_action;
    private int jumpIntent_waitFrameCounter = 0;
    private int jumpIntent_waitFrameTarget;
    private int jumpIntent_stage = 0;
    private void ProcessJumpIntent()
    {
        if(jumpIntent_stage == 0)
        {
            Vector2 agentVelocity = new Vector2(Mathf.Sign(jumpIntent_action.XSpeed)*agent.speed, 0.0f);
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
                SetIntent(Intent.WaitUntilGround, AIFacingDirection.SameAsMoving);
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
            rb.linearVelocity = new Vector2(walkOffEdgeIntent_action.XSpeed, 0.1f);
            walkOffEdgeIntent_stage = 1;
        }
        else
        {
            rb.linearVelocity = new Vector2(walkOffEdgeIntent_action.XSpeed, 0.1f);
            if(
                (walkOffEdgeIntent_action.XSpeed < 0.0f)
                    ?transform.position.x < walkOffEdgeIntent_action.TargetX
                    :transform.position.x > walkOffEdgeIntent_action.TargetX
            )
            {
                SetIntent(Intent.WaitUntilGround, AIFacingDirection.SameAsMoving);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        GameObject otherGameObject = other.gameObject;

        DynamicAIAction jumpBlock;
        if(otherGameObject.TryGetComponent<DynamicAIAction>(out jumpBlock))
        {
            listeningDynamicAction.Add(jumpBlock.Link.name, jumpBlock);
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
            if( intent == Intent.PrepJump && 
                jumpBlock.Type == DynamicAIAction.ActionType.Jump &&
                prepJumpIntent_pendingAction == jumpBlock
            ) {
                SetIntent(Intent.Idle, AIFacingDirection.None, false, Intent.PrepJump);
            }
            listeningDynamicAction.Remove(jumpBlock.Link.name);
        }
    }

    void SafeWarp()
    {
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
            Debug.LogError($"Agent Can't Warp, at {transform.position}"); // TODO: FIXME: you know this is not the solution
        }
    }
}
