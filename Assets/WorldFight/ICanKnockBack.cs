using UnityEngine;
public interface ICanKnockback
{
    public abstract Vector2 KnockbackDir(GameObject other);
    public abstract float KnockbackPower(GameObject other);
    public abstract bool KnockbackStun{get;}
    public abstract bool DoKnockback(GameObject other);

    public abstract void OnCollisionEnter2D(Collision2D collision);
}