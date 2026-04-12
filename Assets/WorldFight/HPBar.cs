using System;
using UnityEngine;
using UnityEngine.Events;

// 在玩家或是敵人上的HP bar

public class HPBar : MonoBehaviour
{
    public RectTransform GreenTrans; // Transform of the green part of the HP bar
    public GameObject Followee; // GameObject to follow
    public Vector3 Offset; // Offset from the followee's transform.positoin
    
    private Vector3 Velocity;
    public float SmoothTime;

    [NonSerialized] public UnityAction OnHpLE0;
    private float _HP = 1.0f;
    private float _maxHP = 1.0f;
    public float HP
    {
        get => _HP;
        set
        {
            if(_maxHP < value)
            {
                value = _maxHP;
            }
            if(_HP <= 0.0f) {return;}

            _HP = value;
            if(_HP <= 0.0f)
            {
                this.OnHpLE0();
                _HP = 0.0f;
            }

            float newScale = _HP / _maxHP;
            float deltaScale = newScale - GreenTrans.localScale.x;
            GreenTrans.localPosition += new Vector3(deltaScale * 0.5f, 0f, 0f);
            Vector3 greenTransLocalScale = GreenTrans.localScale;
            greenTransLocalScale.x = newScale;
            GreenTrans.localScale = greenTransLocalScale;
        }
    }
    public float MaxHP
    {
        get => _maxHP;
        set
        {
            if(_HP > value)
            {
                Debug.LogWarningFormat("Setting maxHP to {0}, which is smaller than hp {1}", _maxHP, _HP);
                _HP = value;
                if(_HP <= 0.0f)
                {
                    this.OnHpLE0();
                }
            }
            _maxHP = value;

            float newScale = _HP / _maxHP;
            float deltaScale = newScale - GreenTrans.localScale.x;
            GreenTrans.localPosition += new Vector3(deltaScale * 0.5f, 0f, 0f);
            Vector3 greenTransLocalScale = GreenTrans.localScale;
            greenTransLocalScale.x = newScale;
            GreenTrans.localScale = greenTransLocalScale;
        }
    }

    public void FixedUpdate()
    {
        // Move toward the followee
        transform.position = Vector3.SmoothDamp(
            transform.position,
            Followee.transform.position + Offset,
            ref Velocity,
            SmoothTime
        );
    }
}
