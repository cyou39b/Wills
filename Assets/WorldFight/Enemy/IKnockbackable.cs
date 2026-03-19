using UnityEngine;
// reference: https://www.youtube.com/watch?v=0NH5obeOb7I 
public interface IKnockbackable
{
    public Rigidbody2D rb{get;}
    public GameObject gameObject{get;}
    public void GetKnockbacked(Vector2 direction, float power, bool stun, Vector2 forcePosition);
}