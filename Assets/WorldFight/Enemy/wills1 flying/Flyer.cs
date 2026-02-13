using System.Collections;
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

        SetIntent(StartCoroutine(ChasePlayer()), false);
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

    private static readonly WaitForSeconds agentUpdateTimeSpan = new WaitForSeconds(0.125f);
    IEnumerator ChasePlayer()
    {
        while(true)
        {
            agent.SetDestination(playerTrans.position);
            Anmor.speed = Mathf.Max(0.0f, agent.velocity.y / 9.81f + 1.0f);
            yield return agentUpdateTimeSpan;
        }
    }

    protected override (float, float, Vector3?) HpBarData() 
        => (40.0f, 40.0f, new Vector3(0.0f, 1.15f, 0.0f));

    public override void GetKnockbacked(Vector2 direction, float power, bool stun)
    {
        SetIntent(StartCoroutine(GetKnockbackedIntent(direction, power, stun)), true);
    }

    private IEnumerator GetKnockbackedIntent(Vector2 direction, float power, bool stun)
    {
        yield return null;
        // Disable Nav Mesh agent and enable rigidbody.
        agent.enabled = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = (stun)?1.0f:0.0f;
        rb.AddForce(direction * power);

        // make sure the force got applied
        yield return new WaitForFixedUpdate();
        // float startTime = Time.time;
        yield return new WaitUntil(() => rb.linearVelocity.magnitude <= StillEpsilon /*|| Time.time - startTime >= 0.5f*/);

        // Enable agent
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        agent.Warp(transform.position);
        agent.enabled = true;

        // make sure that agent got re-enabled
        yield return null;
        SetIntent(StartCoroutine(ChasePlayer()), false);
    }
}
