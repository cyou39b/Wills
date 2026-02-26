using System.Collections;
using System.IO.Compression;
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
                SetIntent(Intent.ChasePlayer, false);
                break;
            case Intent.ChasePlayer:
                ProcessChasePlayerIntent();
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

    private int destinationUpdateCounter = 0;
    private const int destinationUpdateCount = 7;
    void ProcessChasePlayerIntent()
    {
        if(++destinationUpdateCounter != destinationUpdateCount){return;}
        else{destinationUpdateCounter = 0;}

        agent.SetDestination(playerTrans.position);
        Anmor.speed = Mathf.Max(0.0f, agent.velocity.y / 9.81f + 1.0f);
    }

    protected override (float, float, Vector3?) HpBarData() 
        => (40.0f, 40.0f, new Vector3(0.0f, 1.15f, 0.0f));

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

    private Vector2 getKnockbackedIntent_force;
    private bool getKnockbackedIntent_stun;
    private void ProcessGetKnockbackedIntent()
    {
        // Disable Nav Mesh agent and enable rigidbody.
        agent.enabled = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = getKnockbackedIntent_stun?1.0f:0.0f;
        rb.AddForce(getKnockbackedIntent_force);
        SetIntent(Intent.WaitUntilStill, false, Intent.GetKnockbacked);

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
            SetIntent(Intent.Idle, false, Intent.WaitUntilStill);
        }
    }
}
