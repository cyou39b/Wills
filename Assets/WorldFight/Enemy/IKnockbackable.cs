using UnityEngine;
// reference: https://www.youtube.com/watch?v=0NH5obeOb7I 
public interface IKnockbackable
{
    public Rigidbody2D rb{get;} // what?
    public GameObject gameObject{get;} // tf;
    public void GetKnockbacked(Vector2 direction, float power, bool stun);
}