using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Wills : AbstractEnemy
{
    public WillsWeapon weapon;
    public GameObject BulletPrefab;

    public Color[] BodyColors;
    public Color[] EyeColors;

    private const string anmorIsWalkingTag = "isWalking";
    private bool _walking = false;
    private bool walking 
    {
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

    private float _moveSpeed = 5.5f;
    public override float MoveSpeed 
    {
        get => _moveSpeed;
        set
        {
            _moveSpeed = value;
            agent.speed = value;
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

        SafeWarp();

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
        triangle.wills1Color = mainColor;
        Mat.SetColor("_EyeColor", EyeColors[idx]);

        walking = false;
    }

    public override (float, float, Vector3) HpBarData 
        => (40.0f, 40.0f, new Vector3(0.0f, 2.0f, 0.0f));

    public void Update()
    {
        Vector3 dir = agent.enabled
            ?agent.steeringTarget-transform.position
            :rb.linearVelocity;
        switch(AIFacingDirection)
        {
            case AIFacingDirection.SameAsMoving:
                if(dir.x==0.0f) {break;}
                CurrentRealFacingDirection = dir.x<=0.0f
                    ?FacingDirection.Left
                    :FacingDirection.Right;
                break;
            case AIFacingDirection.NegFromMoving:
                if(dir.x==0.0f) {break;}
                CurrentRealFacingDirection = dir.x>=0.0f
                    ?FacingDirection.Left
                    :FacingDirection.Right;
                break;
            case AIFacingDirection.FacingPlayer:
                CurrentRealFacingDirection = playerTrans.position.x > transform.position.x
                    ?FacingDirection.Right
                    :FacingDirection.Left;
                break;
            case AIFacingDirection.NotFacingPlayer:
                CurrentRealFacingDirection = playerTrans.position.x < transform.position.x
                    ?FacingDirection.Right
                    :FacingDirection.Left;
                break;
            case AIFacingDirection.None:
                //* Nothin
                break;
        }
    }

    [ContextMenu("Dir")]
    void g()
    {
        Debug.Log($"{agent.enabled}, {agent.steeringTarget}, {transform.position}, {rb.linearVelocity}");
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if(TrySeePlayer())
        {
            Vector3 dir = playerTrans.position - weapon.transform.position;
            weapon.SetRotation(dir);
        }
        else
        {
            weapon.SetRotation(CurrentRealFacingDirection == FacingDirection.Left ? Vector3.left : Vector3.right);
        }
    }

    protected override FacingDirection CurrentRealFacingDirection
    { 
        get => base.CurrentRealFacingDirection;
        set
        {
            if(value == base.CurrentRealFacingDirection) {return;}

            if(value == FacingDirection.Left)
            {
                SpRr.flipX = true;
            }
            else
            {
                SpRr.flipX = false;
            }
            base.CurrentRealFacingDirection = value; 
        } 
    }

    protected override void MainProcessIntent()
    {
        restart:
        switch(intent)
        {
            case Intent.Idle:
            #if UNITY_EDITOR
                if(!AI){break;}
            #endif
                if(TrySeePlayer())
                {
                    SetIntent(Intent.Attack, AIFacingDirection.FacingPlayer);
                }
                if (!agent.enabled)
                {
                    SafeWarp();
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
            if(value == intent){return;}

            // NOTE: On leaving a intent
            switch(intent)
            {
                case Intent.Jump:
                    rb.linearDamping = 0.25f;
                    break;
                default:
                    break;
            }

            // NOTE: On entering a intent
            switch(value)
            {
                case Intent.ChasePlayer:
                    walking = true;
                    chasePlayer_counter = chasePlayer_count;
                    break;
                case Intent.Attack:
                    walking = false;
                    attack_coolDownTimer = 0;
                    break;
                case Intent.PrepJump:
                    prepJumpIntent_stage = 0;
                    break;
                case Intent.Jump:
                    walking = true;
                    break;
                case Intent.WalkOffEdge:
                    walking = true;
                    walkOffEdgeIntent_stage = 0;
                    break;
                case Intent.WaitUntilGround:
                    waitUntilGround_stage = 0;
                    waitUntilGround_keepXSpeed = float.NaN;
                    break;
            }
            
            base.intent = value;
        } 
    }

    public override void GetKnockbacked(Vector2 direction, float power, bool stun, Vector2 forcePosition)
    {
        getKnockbackedIntent_force += direction * power;
        getKnockbackIntent_position = forcePosition; 
        getKnockbackedIntent_stun |= stun;
        SetIntent(Intent.GetKnockbacked, AIFacingDirection.NegFromMoving, true);
    }

    public float xEpsilon, yEpsilon;
    public float xKnockbackModifier, yKnockbackModifier;
    private Vector2 getKnockbackedIntent_force = Vector2.zero;
    private Vector2 getKnockbackIntent_position = Vector2.zero;
    private bool getKnockbackedIntent_stun = false;
    private void ProcessGetKnockbackedIntent()
    {
        Debug.Log($"{name} get knockback with force {getKnockbackedIntent_force}");
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
                    ((ICanKnockback)this).DoKnockback(
                        knockbackable,
                        normalizeForce,
                        thisSimilarity * power / totalSimilarity,
                        false
                    );
                }
            }
            getKnockbackedIntent_force *= Mathf.Max(0.0f, 1.0f - totalSimilarity * (1.0f-collisionForceKeepRatio));
        }

        getKnockbackedIntent_force.x *= xKnockbackModifier;
        getKnockbackedIntent_force.y *= yKnockbackModifier;
        agent.enabled = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForceAtPosition(getKnockbackedIntent_force, getKnockbackIntent_position);
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
    private float waitUntilGround_keepXSpeed;
    private void ProcessWaitUntilGroundedIntent()
    {
        switch(waitUntilGround_stage)
        {
            case 0:
                if(rb.linearVelocityY == 0.0f) 
                {
                    waitUntilGround_stage = 3;
                    goto case 3;
                }
                else
                {
                    waitUntilGround_stage = 1;
                    goto case 1;
                }
            case 1:
                // wait until it's falling
                if(!float.IsNaN(waitUntilGround_keepXSpeed))
                {
                    rb.linearVelocityX = waitUntilGround_keepXSpeed;
                }
                if(rb.linearVelocityY < 0.0f) 
                {
                    waitUntilGround_stage = 2;
                    goto case 2;
                }
                break;
            case 2:
                // wait while it's still falling
                if(!float.IsNaN(waitUntilGround_keepXSpeed))
                {
                    rb.linearVelocityX = waitUntilGround_keepXSpeed;
                }
                if(rb.linearVelocityY == 0.0f) 
                {
                    waitUntilGround_stage = 3;
                    goto case 3;
                }
                break;
            case 3:
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;

                SafeWarp();
                SetIntent(Intent.Idle, AIFacingDirection.None, false, Intent.WaitUntilGround); // FIXME?: fail to set intent sometimes
                break;
            default:
                Debug.LogError($"Invalid stage number: {waitUntilGround_stage}");
                break;
        }
    }

    private const int chasePlayer_count = 8;
    private int chasePlayer_counter = 0;
    private void ProcessChasePlayerIntent()
    {
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
        if(action != null && action.inRange(transform.position.x))
        {
            switch (action.Type)
            {
                case DynamicAIAction.ActionType.Jump:
                    prepJumpIntent_pendingAction = action;
                    SetIntent(Intent.PrepJump, AIFacingDirection.SameAsMoving);
                    return;
                case DynamicAIAction.ActionType.WalkOffEdge:
                    walkOffEdgeIntent_action = action;
                    SetIntent(Intent.WalkOffEdge, AIFacingDirection.SameAsMoving);
                    return;
            }
        }

        if(++chasePlayer_counter < chasePlayer_count){return;}
        else{chasePlayer_counter = 0;}

        if(TrySeePlayer()) 
        {
            SetIntent(Intent.Attack, AIFacingDirection.SameAsMoving);
            return;
        }

        RaycastHit2D info = Physics2D.Raycast(playerTrans.position, Vector2.down, 100.0f, DefinedLayers.GroundLayerMask);
        agent.SetDestination(info.point);
    }

    [ContextMenu("Log stuffs")]
    public void LogStuffs()
    {
        string listners = string.Join(Environment.NewLine, listeningDynamicAction);
        string ol = agent.isOnOffMeshLink ? agent.currentOffMeshLinkData.owner.name : "False";
        Debug.Log($"name: {name}, intent: {intent}, listeners: {listners}, onlink: {ol}, owner==null: {agent.nextOffMeshLinkData.owner == null}, dest: {agent.destination}");
    }

    private const int seeRayCastMsk = ~0 
        ^DefinedLayers.EnemyLayerMask 
        ^DefinedLayers.AttackLayerMask 
        ^DefinedLayers.NavMeshLayerMask
        ^(1<<2);// (1<<2) is mask for ignore raycast
    public float SeeDistance;
    bool TrySeePlayer()
    {
        Vector3 startPos = weapon.transform.position;
        Vector3 dir = playerTrans.position - startPos;
        RaycastHit2D info = Physics2D.Raycast(startPos, dir, SeeDistance, seeRayCastMsk);
        return info.collider != null &&
               info.collider.gameObject.layer == DefinedLayers.PlayerLayer;
    }

    private const int attack_coolDownTarget = 100;
    private int attack_coolDownTimer = 0;
    private void ProcessAttackIntent()
    {
        if(attack_coolDownTimer++ == 0)
        {
            agent.enabled = false;

            Vector3 dir = playerTrans.position - weapon.transform.position;
            Instantiate(BulletPrefab, weapon.transform.position, Quaternion.Euler(
                    0.0f, 0.0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg
                )
            );
        }
        if(attack_coolDownTimer < attack_coolDownTarget) {return;}

        SetIntent(Intent.Idle, AIFacingDirection.None, false, Intent.Attack);
    }

    private DynamicAIAction prepJumpIntent_pendingAction = null;
    private Vector2 prepJumpIntent_pendingVelocity;
    private int prepJumpIntent_stage = 0;
    private int prepJumpIntent_waitFrameTimer = 0;
    private int prepJumpIntent_waitFrameTarget = 0;
    private void ProcessPrepJumpIntent()
    {
        if(prepJumpIntent_stage == 0)
        {
            prepJumpIntent_pendingVelocity = new Vector2(
                Mathf.Sign(
                    prepJumpIntent_pendingAction.EndZoneRight > prepJumpIntent_pendingAction.StartZoneRight
                        ?prepJumpIntent_pendingAction.StartZoneRight - transform.position.x
                        :prepJumpIntent_pendingAction.StartZoneLeft - transform.position.x
                ) * agent.speed, 
                0.0f
            );
            agent.enabled = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1.0f;
            rb.linearVelocity = prepJumpIntent_pendingVelocity;

            prepJumpIntent_waitFrameTimer = 0;
            prepJumpIntent_waitFrameTarget = Random.Range(0, prepJumpIntent_pendingAction.TimeRange);
            prepJumpIntent_stage = 1;
        }
        else
        {
            if(++prepJumpIntent_waitFrameTimer < prepJumpIntent_waitFrameTarget) {return;}
            jumpIntent_action = prepJumpIntent_pendingAction;
            SetIntent(Intent.Jump, AIFacingDirection.SameAsMoving);
        }
    }

    private DynamicAIAction jumpIntent_action;
    private void ProcessJumpIntent()
    {
        agent.enabled = false;

        JumpFunction jf;
        (string op_err, JumpFunction? op_jf) = MathUtil.CalculateJumpCurveWithRange(
            transform.position, 
            jumpIntent_action.StartZoneLeft, 
            jumpIntent_action.StartZoneRight,
            jumpIntent_action.Link.transform.rotation * jumpIntent_action.Link.endPoint + jumpIntent_action.Link.transform.position,
            jumpIntent_action.EndZoneLeft,
            jumpIntent_action.EndZoneRight,
            jumpIntent_action.JumpHighestPoint
        );
        if(op_jf == null)
        {
            Debug.LogError(op_err);
            return;
        }
        else {jf = op_jf.Value;}

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1.0f;
        rb.linearDamping = 0.0f;
        rb.linearVelocity = new Vector2(
            jf.xt.velocity,
            jf.yt.b
        );
        SetIntent(Intent.WaitUntilGround, AIFacingDirection.SameAsMoving);
        waitUntilGround_keepXSpeed = jf.xt.velocity;
    }

    private DynamicAIAction walkOffEdgeIntent_action;
    private float walkOffEdgeIntent_dir;
    private int walkOffEdgeIntent_stage = 0;
    private void ProcessWalkOffEdgeIntent()
    {
        if(walkOffEdgeIntent_stage == 0)
        {
            walkOffEdgeIntent_dir = Mathf.Sign(walkOffEdgeIntent_action.TargetX - transform.position.x);
            
            agent.enabled = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1.0f;
            rb.linearVelocity = new Vector2(walkOffEdgeIntent_action.XSpeed * walkOffEdgeIntent_dir, 0.001f);
            walkOffEdgeIntent_stage = 1;
        }
        else
        {
            rb.linearVelocity = new Vector2(walkOffEdgeIntent_action.XSpeed * walkOffEdgeIntent_dir, 0.001f);
            if(
                (walkOffEdgeIntent_dir < 0.0f)
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

        ConnectionDatas datas;
        if(otherGameObject.layer == DefinedLayers.NavMeshLayer && otherGameObject.TryGetComponent<ConnectionDatas>(out datas))
        {
            foreach(DynamicAIAction action in datas.Actions)
            {
                listeningDynamicAction.TryAdd(action.Link.name, action);
            }
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

        ConnectionDatas datas;
        if(otherGameObject.layer == DefinedLayers.NavMeshLayer && otherGameObject.TryGetComponent<ConnectionDatas>(out datas))
        {
            foreach(DynamicAIAction action in datas.Actions)
            {
                listeningDynamicAction.Remove(action.Link.name);
            }
        }
    }

    void SafeWarp()
    {
        if(agent.enabled && agent.isOnNavMesh) {return;}
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
        agent.enabled = true;
    }
}
