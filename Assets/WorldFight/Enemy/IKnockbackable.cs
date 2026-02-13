using UnityEngine;

// reference: https://www.youtube.com/watch?v=0NH5obeOb7I 
public interface IKnockbackable
{
    [SerializeField] public ParticleSystem.MinMaxCurve DistanceToKnockbackPowerCurve{get;set;}
    public void GetKnockbacked(Vector2 direction, float power, bool stun);
}