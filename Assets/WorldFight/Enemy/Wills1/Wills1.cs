using System.Collections;
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
    
    public float xForceModify;
    public float yForceModify;
    public float MoveSpeed;
    private FacingDirection dir = FacingDirection.None;

    private NavMeshAgent agent;
    [SerializeField] private float stillEpsilon;

    public GameObject FireworkPrefab;

    protected override void Start()
    {
        base.Start();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        // agent.autoTraverseOffMeshLink = false;

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

        SetIntent(StartCoroutine(IdleIntent()), false);
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
                SpRr.flipX = targetDirection.x <= 0.0f;
            }
        }
        else
        {
            SpRr.flipX = rb.linearVelocity.x <= 0.0f;
        }
    }

    public override void GetKnockbacked(Vector2 direction, float power, bool stun)
    {
        SetIntent(StartCoroutine(GetKnockbackedIntent(direction, power, stun)), true);
    }

    private IEnumerator GetKnockbackedIntent(Vector2 direction, float power, bool stun)
    {
        yield return null;

        agent.enabled = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1.0f;
        rb.AddForce(direction * power);

        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(()=>rb.linearVelocity.magnitude <= stillEpsilon);
        // TODO: Make xEpsilon and yEpsilon

        SetIntent(StartCoroutine(WaitUntilGroundedIntent()), false);
    }

    private IEnumerator WaitUntilGroundedIntent()
    {
        if(rb.linearVelocityY != 0.0f)
        {
            // wait until it's falling
            yield return new WaitUntil(()=>rb.linearVelocityY < 0.0f);
        }

        // wait while it's still falling BUGGY?
        yield return new WaitWhile(()=>rb.linearVelocityY != 0.0f);

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        // wait for that shit on the previous line to be applied
        yield return new WaitForFixedUpdate();

        if(!agent.Warp(transform.position))
        {
            NavMeshQueryFilter filter = new NavMeshQueryFilter()
            {
                agentTypeID = agent.agentTypeID,
                areaMask = NavMesh.AllAreas
            };
            NavMeshHit hit;

            if(NavMesh.SamplePosition(transform.position, out hit, 1.0f, filter)) // WARN: Magic number
            {
                agent.Warp(hit.position);
            }
            else
            {
                Debug.LogError($"Agent Can't Warp, at {transform.position}");
            }
        }
        agent.enabled = true;

        yield return null;
        SetIntent(StartCoroutine(IdleIntent()), false);
    }

    // The intent when the AI have nothing to do.
    private IEnumerator IdleIntent()
    {
        yield return null;
        SetIntent(StartCoroutine(ChasePlayer()), false);
    }

    private static readonly WaitForSeconds agentUpdateTimeSpan = new WaitForSeconds(0.125f);
    private IEnumerator ChasePlayer()
    {
        while(true)
        {
            agent.SetDestination(playerTrans.position);
            yield return agentUpdateTimeSpan;
        }
    }
}
