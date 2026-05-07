using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Jack))]
public class JackEntrance : MonoBehaviour
{
    public Vector2 startVelocity;
    public float gravityScaleWithParachute;
    private Rigidbody2D rb;

    private Jack jack;

    private GameObject parachute;
    private Animator parachuteAnimator;
    public RuntimeAnimatorController parachuteLandAnimation;

    void Start()
    {
        jack = GetComponent<Jack>();
        jack.enabled = false;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScaleWithParachute;

        Transform parachute = transform.Find("Parachute");
        if(parachute == null)
        {
            Debug.LogError("Missing child GameObject.");
        }
        this.parachute = parachute.gameObject;

        if(!parachute.TryGetComponent<Animator>(out parachuteAnimator))
        {
            Debug.LogError("Missing component.");
        }
    }

    [System.NonSerialized] public bool called = true;
    void FixedUpdate()
    {
        if(!called && rb.linearVelocityY == 0.0f)
        {
            jack.enabled = true;
            jack.StartPlayerAction();
            RbCameraMovement.UseRB = true;
            StartCoroutine(CleanupParachute());
            called = true;
            return;
        }
    }

    private const float parachuteLandAnimationTime = 1.4f;
    IEnumerator CleanupParachute()
    {
        parachute.AddComponent<Rigidbody2D>();
        parachute.transform.parent = null;
        parachuteAnimator.runtimeAnimatorController = parachuteLandAnimation;

        yield return new WaitForSeconds(parachuteLandAnimationTime);

        rb.gravityScale = 1.0f;
        Destroy(parachute);
        Destroy(this);
    }
}
