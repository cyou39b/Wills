using UnityEngine;
using System.Collections.Generic;
using System;

public class JacLeg : MonoBehaviour
{
    [NonSerialized] public Dictionary<GameObject, IKnockbackable> objectsStandingOn = new Dictionary<GameObject, IKnockbackable>();

    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject other = collider.gameObject;
        if(other.layer == DefinedLayers.GroundLayer)
        {
            objectsStandingOn[other] = null;
        }
        else if(other.layer == DefinedLayers.EnemyLayer)
        {
            IKnockbackable knockbackable = other.GetComponent<IKnockbackable>();
            objectsStandingOn[other] = knockbackable;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        GameObject other = collider.gameObject;
        if(other.layer == DefinedLayers.EnemyLayer || other.layer == DefinedLayers.GroundLayer)
        {
            objectsStandingOn.Remove(other);
        }
    }
}
