using UnityEngine;
using System.Collections.Generic;

public interface ICanKnockback
{
    public abstract Rigidbody2D rb{get;}
    public abstract GameObject gameObject{get;}
    public abstract float collisionForceGiveRatio{get;}
    public abstract float collisionForceKeepRatio{get;}
    public void DoKnockback(IKnockbackable knockbackable, Vector2 dir, float power, bool stun)
    {
        List<Collider2D> contacts = new List<Collider2D>();
        rb.GetContacts(contacts);

        foreach(Collider2D contact in contacts)
        {
            if(contact.gameObject != knockbackable.gameObject) {continue;}
            
            Debug.Log($"{gameObject.name} knockback {knockbackable.gameObject.name}");
            knockbackable.GetKnockbacked(
                dir, 
                power * collisionForceGiveRatio, 
                stun, 
                contact.ClosestPoint(rb.position)
            );

            rb.linearVelocity *= collisionForceKeepRatio;
            break;
        }
    }
}