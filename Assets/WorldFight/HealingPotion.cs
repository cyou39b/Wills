using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class HealingPotion : MonoBehaviour
{
    Rigidbody2D rb = null;
    Vector2? velocity = null;

    CircleCollider2D effectRange;
    List<IHaveHP> haveHPInRange = new List<IHaveHP>();
    float healAmount;
    public float startDistance;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if(velocity != null) {rb.linearVelocity = velocity.Value;}
        rb.angularVelocity = Random.Range(-1.0f, 1.0f);

        effectRange = GetComponent<CircleCollider2D>();
    }

    public void Initialize(float healAmount, Vector2 velocity)
    {
        if(rb != null) {rb.linearVelocity = velocity;}
        else {this.velocity = velocity;}
        this.healAmount = healAmount;
        transform.position = MathUtil.AddVectors(transform.position, velocity.normalized * startDistance);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach(IHaveHP haveHP in haveHPInRange)
        {
            haveHP.HPBar.HP += healAmount;
        }

        Destroy(gameObject);
        return;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        IHaveHP haveHP;
        if(collision.TryGetComponent<IHaveHP>(out haveHP))
        {
            haveHPInRange.Add(haveHP);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        IHaveHP haveHP;
        if(collision.TryGetComponent<IHaveHP>(out haveHP))
        {
            haveHPInRange.Remove(haveHP);
        }
    }
}
