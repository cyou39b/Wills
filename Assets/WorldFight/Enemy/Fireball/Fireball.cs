using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
public class Fireball : MonoBehaviour
{
    private new Collider2D collider;
    private Rigidbody2D rb;
    private Animator animator;
    private const string animatorDissipateParameterName = "Dissipate";

    public GameObject fireEffectPrefab;
    void Start()
    {
        collider = GetComponent<Collider2D>();
        collider.forceReceiveLayers = 0;
        collider.forceSendLayers = 0;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private const int goFrame = 30;
    public float InitializeDistance;
    private GameObject player;
    public void Initialize(GameObject player, Vector3 targetPos)
    {
        this.player = player;
        Vector3 dPos = targetPos - transform.position;

        transform.SetPositionAndRotation(
            transform.position + dPos.normalized * InitializeDistance,
            transform.rotation = Quaternion.Euler(
                0.0f,
                0.0f,
                Mathf.Atan2(dPos.y, dPos.x) * Mathf.Rad2Deg
            )
        );

        StartCoroutine(GO());
    }

    public float Speed;
    IEnumerator GO()
    {
        for(int _=0;_<goFrame;_++){yield return new WaitForFixedUpdate();}

        if(this == null || gameObject == null) {yield break;}
        transform.parent = null;
        collider.forceReceiveLayers = ~0;
        collider.forceSendLayers = ~0;
        float rot = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        rb.linearVelocity = new Vector3(Mathf.Cos(rot), Mathf.Sin(rot), 0.0f) * Speed;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject other = collider.gameObject;
        if(
            other.layer == DefinedLayers.GroundLayer || 
            other.layer == DefinedLayers.WallLayer   ||
            other.layer == DefinedLayers.PlayerLayer )
        {
            GameObject newObj = Instantiate(fireEffectPrefab, transform.position + 0.6f*transform.right, Quaternion.identity);
            Transform fireEffectColliderTrans = newObj.transform.Find("Collider");
            if(fireEffectColliderTrans == null) {Debug.LogError("Missing child object");}

            FireEffectCollider fireEffectCollider;
            if(!fireEffectColliderTrans.TryGetComponent<FireEffectCollider>(out fireEffectCollider)){Debug.LogError("Missing Component (Monobehaviour script)");}

            fireEffectCollider.player = player;
            Destroy(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Field"))
        {
            Destroy(gameObject);
            return;
        }
    }
}
