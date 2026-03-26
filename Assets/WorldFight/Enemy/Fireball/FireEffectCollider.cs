using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D), typeof(Rigidbody2D))]
public class FireEffectCollider : MonoBehaviour, ICanKnockback
{
    [NonSerialized] public GameObject fireballSpawner;
    private new CapsuleCollider2D collider;
    private GameObject parent;

    private Rigidbody2D rb;
    Rigidbody2D ICanKnockback.rb => rb;
    GameObject ICanKnockback.gameObject => gameObject;
    float ICanKnockback.collisionForceGiveRatio => 1.0f;
    float ICanKnockback.collisionForceKeepRatio => 0.0f;

    private static readonly Vector3 animationOffset = new Vector3(0.0f, 0.33f, 0.0f);
    private static readonly Vector3 startScale = new Vector3(0.48f, 0.48f, 1.0f);
    private static readonly Vector3 endScale = new Vector3(1.0f, 1.0f, 1.0f);
    private static readonly Vector3 startPos = new Vector3(0.0f, -0.2f, 0.0f);
    private static readonly Vector3 endPos = new Vector3(0.0f, 0.0f, 0.0f);
    IEnumerator Start()
    {
        collider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        parent = transform.parent.gameObject;
        parent.transform.position += animationOffset;
        transform.localScale = startScale;
        transform.localPosition = startPos;
        
        float timer = 30.0f/48.0f, totalTime = timer;
        while(timer > 0.0f)
        {
            yield return null;
            timer -= Time.deltaTime;

            transform.localScale = Vector3.Lerp(endScale, startScale, timer/totalTime);
            transform.localPosition = Vector3.Lerp(endPos, startPos, timer/totalTime);
        }
        Destroy(parent);
    }

    public float Power = 750.0f;
    public void OnTriggerStay2D(Collider2D collider)
    {
        GameObject other = collider.gameObject;
        IKnockbackable knockbackable;
        if(
            other != fireballSpawner && 
            other.TryGetComponent<IKnockbackable>(out knockbackable)
          )
        {
            ((ICanKnockback)this).DoKnockback(
                knockbackable,
                other.transform.position - transform.position,
                Power,
                false
            );
        }
    }
}
