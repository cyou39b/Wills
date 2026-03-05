using UnityEngine;
public interface ICanKnockback
{
    public abstract Vector2 KnockbackDir{get;}
    public abstract float KnockbackPower{get;}
    public abstract bool KnockbackStun{get;}
    public abstract bool DoKnockback(GameObject other);

    public abstract void OnCollisionEnter2D(Collision2D collision);
}