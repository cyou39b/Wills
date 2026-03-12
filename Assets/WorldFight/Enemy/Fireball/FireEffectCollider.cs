using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class FireEffectCollider : MonoBehaviour, ICanKnockback
{
    private new CapsuleCollider2D collider;
    private GameObject parent;
    private static readonly Vector3 animationOffset = new Vector3(0.0f, 0.33f, 0.0f);
    private static readonly Vector3 startScale = new Vector3(0.48f, 0.48f, 1.0f);
    private static readonly Vector3 endScale = new Vector3(1.0f, 1.0f, 1.0f);
    private static readonly Vector3 startPos = new Vector3(0.0f, -0.2f, 0.0f);
    private static readonly Vector3 endPos = new Vector3(0.0f, 0.0f, 0.0f);
    IEnumerator Start()
    {
        collider = GetComponent<CapsuleCollider2D>();
        parent = transform.parent.gameObject;
        parent.transform.position += animationOffset;
        transform.localScale = startScale;
        transform.localPosition = startPos;
        
        float timer = 30.0f/36.0f, totalTime = timer;
        while(timer > 0.0f)
        {
            yield return null;
            timer -= Time.deltaTime;

            transform.localScale = Vector3.Lerp(endScale, startScale, timer/totalTime);
            transform.localPosition = Vector3.Lerp(endPos, startPos, timer/totalTime);
        }
        Destroy(parent);
    }

    public bool DoKnockback(GameObject other)
        => true;
    public Vector2 KnockbackDir(GameObject other)
        => other.transform.position - transform.position - animationOffset;
    public float KnockbackPower(GameObject other)
        => 2000.0f;
    public bool KnockbackStun => false;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;
        IKnockbackable knockbackable;
        Debug.Log(other.name);
        if(other.TryGetComponent<IKnockbackable>(out knockbackable))
        {
            if (DoKnockback(other))
            {
                knockbackable.GetKnockbacked(KnockbackDir(other), KnockbackPower(other), KnockbackStun);
            }
        }
    }
}
