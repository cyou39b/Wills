using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
public class Fireball : MonoBehaviour
{
    private new Collider2D collider;
    private Rigidbody2D rb;
    private Animator animator;
    private const string animatorDissipateParameterName = "Dissipate";

    public GameObject fireEffectPrefab;
    void Start()
    {
        collider = GetComponent<Collider2D>();
        collider.forceReceiveLayers = 0;
        collider.forceSendLayers = 0;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public float InitializeDistance;
    private GameObject spawner;
    public void Initialize(GameObject spawner, Vector3 targetPos)
    {
        this.spawner = spawner;
        Vector3 dPos = targetPos - transform.position;

        transform.SetPositionAndRotation(
            transform.position + dPos.normalized * InitializeDistance,
            transform.rotation = Quaternion.Euler(
                0.0f,
                0.0f,
                Mathf.Atan2(dPos.y, dPos.x) * Mathf.Rad2Deg
            )
        );
    }

    public void Cancel()
    {
        if(this == null || gameObject == null) {return;} // FIXME:
        StartCoroutine(dissipate());
    }

    private static readonly WaitForSeconds dissipateTimeSpan = new WaitForSeconds(17f/48f);
    private IEnumerator dissipate()
    {
        animator.SetBool(animatorDissipateParameterName, true);
        yield return dissipateTimeSpan;
        Destroy(gameObject);
    }

    public float Speed;
    public void GO()
    {
        if(this == null) {return;} // FIXME: GO was called after destroyed sometimes.

        transform.parent = null;
        collider.forceReceiveLayers = ~0;
        collider.forceSendLayers = ~0;
        float rot = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        rb.linearVelocity = new Vector3(Mathf.Cos(rot), Mathf.Sin(rot), 0.0f) * Speed;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;
        if(
            other.layer == DefinedLayers.GroundLayer || 
            other.layer == DefinedLayers.WallLayer   ||
            other.layer == DefinedLayers.PlayerLayer )
        {
            GameObject newObj = Instantiate(fireEffectPrefab, transform.position + 0.6f*transform.right, Quaternion.identity);
            foreach(Transform childTransform in newObj.transform)
            {
                FireEffectCollider fireEffectCollider;
                if(newObj.TryGetComponent<FireEffectCollider>(out fireEffectCollider))
                {
                    fireEffectCollider.fireballSpawner = spawner;
                    break;
                }
            }
            Destroy(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Field"))
        {
            Destroy(gameObject);
            return;
        }
    }
}
