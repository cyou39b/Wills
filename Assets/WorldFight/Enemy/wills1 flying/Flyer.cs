using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D), typeof(NavMeshAgent))]
public class Flyer : AbstractEnemy
{
    public Color[] FaceColors;
    public Color[] FaceOutlineColors;
    public Color[] EyesColors;
    public Color[] WingBColors;
    public Color[] WingGColors;
    public Color[] WingYColors;

    [SerializeField] private float StillEpsilon;

    private float _moveSpeed = 6.0f;
    public override float MoveSpeed 
    {
        get => _moveSpeed;
        set
        {
            _moveSpeed = value;
            agent.speed = value;
        }
    }

   private NavMeshAgent agent;

    public override (float, float, Vector3) HpBarData 
        => (40.0f, 40.0f, new Vector3(0.0f, 1.15f, 0.0f));

    protected override void Start()
    {
        base.Start();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        Material mat = SpRr.material;
        if(mat == null)
        {
            Debug.LogError("material is null.");
        }

        int idx = Random.Range(0, FaceColors.Length);
        mainColor = RandColor();
        mat.SetColor("_Eyes_Color", RandColor());
        mat.SetColor("_Face_Color", mainColor);
        mat.SetColor("_Face_Outline_Color", RandColor());
        mat.SetColor("_WingB_Color", RandColor());
        mat.SetColor("_WingG_Color", RandColor());
        mat.SetColor("_WingY_Color", RandColor());
    }

    Color RandColor()
    {
        return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 0.0f);
    }

    void Update()
    {
        Vector3 dir = agent.enabled
            ?agent.steeringTarget - transform.position
            :rb.linearVelocity;
        switch(AIFacingDirection)
        {
            case AIFacingDirection.SameAsMoving:
                CurrentRealFacingDirection = dir.x <= 0.0f
                    ?FacingDirection.Left
                    :FacingDirection.Right;
                break;
            case AIFacingDirection.NegFromMoving:
                CurrentRealFacingDirection = dir.x >= 0.0f
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
        };
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
                renderingChildObject.transform.localPosition = new Vector3( 0.55f, 0.36f, 0.0f);
            }
            else
            {
                SpRr.flipX = false;
                renderingChildObject.transform.localPosition = new Vector3(-0.55f, 0.36f, 0.0f);
            }
            base.CurrentRealFacingDirection = value;
        }
    }

    protected override void MainProcessIntent()
    {
        switch(intent)
        {
            case Intent.Idle:
                if(!AI){return;}
                if(TrySeePlayer())
                {
                    SetIntent(Intent.Attack, AIFacingDirection.FacingPlayer);
                }
                SetIntent(Intent.ChasePlayer, AIFacingDirection.SameAsMoving);
                break;
            case Intent.RandomlyRoam:
                ProcessRandomlyRoam();
                break;
            case Intent.ChasePlayer:
                ProcessChasePlayerIntent();
                break;
            case Intent.Attack:
                ProccesAttackIntent();
                break;
            case Intent.WaitUntilStill:
                ProcessWaitUntilStillIntent();
                break;
            case Intent.GetKnockbacked:
                ProcessGetKnockbackedIntent();
                break;
            case Intent.EnumCount:
                Debug.LogError("Don't use EnumCount!!");
                break;
            default:
                Debug.LogError($"{nameof(Flyer)} doesn't implement case {intent} for MainProccesIntent");
                break;
        }
    }

    public override Intent intent 
    { 
        get => base.intent;
        set
        {
            if(value == intent){return;}

            switch(value)
            {
                case Intent.ChasePlayer:
                    randomlyRoam_setDestionationLock = false;
                    break;
                case Intent.Attack:
                    attackIntent_chargingFireball = null;
                    attackIntent_fireballChargeFrameCounter = 0;
                    break;
                case Intent.RandomlyRoam:
                    randomlyRoam_setDestionationLock = false;
                    randomlyRoam_coolDownTimer = 0;
                    break;
            }
            base.intent = value;
        }
    }

    public float SeeDistance = 3.0f;
    private const int seeRayCastMsk = ~0 
        ^DefinedLayers.EnemyLayerMask 
        ^DefinedLayers.AttackLayerMask 
        ^DefinedLayers.NavMeshLayerMask
        ^(1<<2);// (1<<2) is mask for ignore raycast
    bool TrySeePlayer()
    {
        RaycastHit2D info = Physics2D.Raycast(transform.position, playerTrans.position-transform.position, SeeDistance, seeRayCastMsk);
        return  info.collider != null && 
                info.collider.gameObject == player;
    }   

    private int destinationUpdateCounter = 0;
    private const int destinationUpdateCount = 7;
    void ProcessChasePlayerIntent()
    {
        if(++destinationUpdateCounter != destinationUpdateCount){return;}
        else{destinationUpdateCounter = 0;}

        agent.SetDestination(playerTrans.position);
        Anmor.speed = Mathf.Max(0.0f, agent.velocity.y / 9.81f + 1.0f);

        if(TrySeePlayer())
        {
            SetIntent(Intent.Attack, AIFacingDirection.FacingPlayer);
        }
    }

    public float roamRadius;
    public float roamStoppingVelocity;
    private bool randomlyRoam_setDestionationLock;
    private const int randomlyRoam_coolDownTarget = 5;
    private int randomlyRoam_coolDownTimer = 0;
    void ProcessRandomlyRoam()
    {
        if(++randomlyRoam_coolDownTimer < randomlyRoam_coolDownTarget) {return;}
        if (!randomlyRoam_setDestionationLock)
        {
            agent.SetDestination(MathUtil.RandomPointInDonut(playerTrans.position, SeeDistance, roamRadius));
            randomlyRoam_setDestionationLock = true;
        }

        if(
            !agent.pathPending && 
            agent.remainingDistance <= agent.stoppingDistance &&
            agent.desiredVelocity.magnitude <= roamStoppingVelocity
        ) {
            SetIntent(Intent.Idle, AIFacingDirection.SameAsMoving, false, Intent.RandomlyRoam);
        }
    }

    public GameObject FireballPrefab;
    private const int attackIntent_FireballChargeTargetFrame = 30;
    private int attackIntent_fireballChargeFrameCounter = 0;
    private Fireball attackIntent_chargingFireball = null;
    void ProccesAttackIntent()
    {
        if(attackIntent_fireballChargeFrameCounter == 0)
        {
            agent.SetDestination(transform.position); // NOTE: assumes that agent is enabled && is on NavMesh

            GameObject newObj = Instantiate(FireballPrefab, transform);
            if(!newObj.TryGetComponent<Fireball>(out attackIntent_chargingFireball))
            {
                Debug.LogError("Fireball no Fireball");
            }
            attackIntent_chargingFireball.Initialize(player, playerTrans.position);
        }
        if(attackIntent_fireballChargeFrameCounter++ >= attackIntent_FireballChargeTargetFrame) {attackIntent_fireballChargeFrameCounter = 0;}
        else{return;}
        SetIntent(Intent.RandomlyRoam, AIFacingDirection.SameAsMoving, false, Intent.Attack);
    }

    public override void GetKnockbacked(Vector2 direction, float power, bool stun, Vector2 forcePosition)
    {
        getKnockbackedIntent_force += direction * power;
        getKnockbackIntent_position = forcePosition;
        getKnockbackedIntent_stun |= stun;
        SetIntent(Intent.GetKnockbacked, AIFacingDirection.NegFromMoving, true);
    }

    private Vector2 getKnockbackedIntent_force = Vector2.zero;
    private Vector2 getKnockbackIntent_position = Vector2.zero;
    private bool getKnockbackedIntent_stun;
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
                        power * thisSimilarity / totalSimilarity,
                        false
                    );
                }
            }
            getKnockbackedIntent_force *= Mathf.Max(0.0f, 1.0f - totalSimilarity * (1.0f-collisionForceKeepRatio));
        }

        agent.enabled = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = getKnockbackedIntent_stun?1.0f:0.0f;
        rb.AddForceAtPosition(getKnockbackedIntent_force, getKnockbackIntent_position);
        SetIntent(Intent.WaitUntilStill, AIFacingDirection.NegFromMoving, false, Intent.GetKnockbacked);

        getKnockbackedIntent_force = Vector2.zero;
        getKnockbackedIntent_stun = false;
    }

    private void ProcessWaitUntilStillIntent()
    {
        if(rb.linearVelocity.magnitude <= StillEpsilon)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            agent.Warp(transform.position);
            agent.enabled = true;
            SetIntent(Intent.Idle, AIFacingDirection.None, false, Intent.WaitUntilStill);
        }
    }
}