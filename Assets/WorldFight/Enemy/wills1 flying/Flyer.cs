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

    private NavMeshAgent agent;

    protected override (float, float, Vector3) HpBarData 
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
        mainColor = FaceColors[idx];
        mat.SetColor("_Eyes_Color", EyesColors[idx]);
        mat.SetColor("_Face_Color", FaceColors[idx]);
        mat.SetColor("_Face_Outline_Color", FaceOutlineColors[idx]);
        mat.SetColor("_WingB_Color", WingBColors[idx]);
        mat.SetColor("_WingG_Color", WingGColors[idx]);
        mat.SetColor("_WingY_Color", WingYColors[idx]);
    }

    void Update()
    {
        Debug.DrawRay(transform.position, playerTrans.position - transform.position, Color.green);
        if(!isIntentPassive)
        {
            // not passive & agent not enabled => in attack/dodge intent and
            // direction is handled in intent coroutine
            if(agent.enabled)
            {
                Vector3 targetDirection = agent.steeringTarget - transform.position;
                SpRr.flipX = targetDirection.x <= 0.0f;
            }
        }
        else
        {
            SpRr.flipX = rb.linearVelocity.x <= 0.0f;
        }
    }

    protected override void MainProcessIntent()
    {
        switch(intent)
        {
            case Intent.Idle:
                if(trySeePlayer())
                {
                    SetIntent(Intent.Attack, AIFacingDirection.SameAsMoving);
                }
                SetIntent(Intent.ChasePlayer, AIFacingDirection.SameAsMoving);
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

    public float SeeDistance = 3.0f;
    private const int seeRayCastMsk = ~0 ^GlobalVariables.EnemyLayerMask ^(1<<2); // (1<<2) is mask for ignore raycast
    bool trySeePlayer()
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

        if(trySeePlayer())
        {
            SetIntent(Intent.Attack, AIFacingDirection.SameAsMoving);
        }
    }

    public GameObject FireballPrefab;
    void ProccesAttackIntent()
    {
        Instantiate(FireballPrefab, transform.position, transform.rotation);
        SetIntent(Intent.Idle, AIFacingDirection.None, false, Intent.Attack);
    }

    public override void GetKnockbacked(Vector2 direction, float power, bool stun)
    {
        getKnockbackedIntent_force += direction * power;
        getKnockbackedIntent_stun |= stun;
        SetIntent(Intent.GetKnockbacked, AIFacingDirection.NegFromMoving, true);
    }

    private Vector2 getKnockbackedIntent_force;
    private bool getKnockbackedIntent_stun;
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
        rb.gravityScale = getKnockbackedIntent_stun?1.0f:0.0f;
        rb.AddForce(getKnockbackedIntent_force);
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
