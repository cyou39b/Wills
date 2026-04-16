using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;

public class JacLeg : MonoBehaviour
{
    [NonSerialized] public Dictionary<GameObject, IKnockbackable> objectsStandingOn = new Dictionary<GameObject, IKnockbackable>();

    [ContextMenu("All contacts")]
    void l()
    {
        StringBuilder sb = new StringBuilder("Dictionary: {");
        foreach(KeyValuePair<GameObject, IKnockbackable> kv in objectsStandingOn)
        {
            sb.Append(kv.Key.name);
            sb.Append(", ");
        }
        sb.Append("}");
        Debug.Log(sb.ToString());
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject other = collider.gameObject;
        if(other.layer == DefinedLayers.EnemyLayer || other.layer == DefinedLayers.GroundLayer)
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
