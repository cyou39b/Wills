using UnityEngine;

// 在玩家或是敵人上的HP bar

public class HPBar : MonoBehaviour
{
    public RectTransform GreenTrans; // Transform of the green part of the HP bar
    public GameObject Followee; // GameObject to follow
    public Vector3 Offset; // Offset from the followee's transform.positoin
    // public Rigidbody2D Rgd;
    public Vector3 Velocity;
    public float SmoothTime;

    public float HP;
    public float MaxHP;

    public void SetHP(float toValue)
    {
        HP = toValue;

        float newScale = HP/MaxHP;
        float deltaScale = newScale - GreenTrans.localScale.x;
        GreenTrans.localPosition += new Vector3(deltaScale * 0.5f, 0f, 0f);
        Vector3 greenTransLocalScale = GreenTrans.localScale;
        greenTransLocalScale.x = newScale;
        GreenTrans.localScale = greenTransLocalScale;
    }
    public void SetMaxHP(float toValue)
    {
        MaxHP = toValue;
        float newScale = HP/MaxHP;
        float deltaScale = newScale - GreenTrans.localScale.x;
        GreenTrans.localPosition += new Vector3(deltaScale * 0.5f, 0f, 0f);
        Vector3 greenTransLocalScale = GreenTrans.localScale;
        greenTransLocalScale.x = newScale;
        GreenTrans.localScale = greenTransLocalScale;
    }

    public void FixedUpdate()
    {
        // Add a force toward followee
        transform.position = Vector3.SmoothDamp(
            transform.position,
            Followee.transform.position + Offset,
            ref Velocity,
            SmoothTime
        );
    }
}
