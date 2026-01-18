using UnityEngine;

public class Wills : MonoBehaviour
{
    public Animator Anmor;
    private bool walking = false;
    private static readonly string anmorIsWalkingTag="isWalking";
    
    public float xForceModify;
    public float yForceModify;
    public float MoveForce;
    public Rigidbody2D Rgd;
    public SpriteRenderer SpRerr;

    public GameObject HpBarPrefab;
    public HPBar HpBar;
    void Start()
    {
        // initialize wills 的HP Bar
        GameObject hpBarGameObject = Instantiate(HpBarPrefab, transform.position, transform.rotation);
        if(!hpBarGameObject.TryGetComponent<HPBar>(out HpBar))
        {
            Debug.LogErrorFormat("Failed to get HPBar component from gameObject created for GameObject named {}.", this.name);
        }
        HpBar.Followee = gameObject;
        HpBar.Offset = new Vector3(0f, 1.2f, 0f);
        HpBar.SetMaxHP(100.0f);
        HpBar.SetHP(100.0f);
    }

    void FixedUpdate()
    {
        // 暫時的移動邏輯
        if(transform.position.x >= 1.0f)
        {
            Rgd.AddForceX(-MoveForce);
        }
        else if(transform.position.x <= -1.0f)
        {
            Rgd.AddForceX(MoveForce);
        }
        else
        {
            Rgd.AddForce(-Rgd.linearVelocity);
        }
        
    }
    void Update()
    {
        // Anmor.SetBool(anmorIsWalkingTag, true); <- 把wills1的動畫調成正在移動
        // Anmor.SetBool(anmorIsWalkingTag, false); <- 把動畫調成站著不動

        if(transform.position.x >= 1.0f)
        {
            SpRerr.flipX = true;
            if (!walking)
            {
                walking = true;
                Anmor.SetBool(anmorIsWalkingTag, true);
            }
        }
        else if(transform.position.x <= -1.0f)
        {
            SpRerr.flipX = false;
            if (!walking)
            {
                walking = true;
                Anmor.SetBool(anmorIsWalkingTag, true);
            }
        }
        else
        {
            if (walking)
            {
                walking = false;
                Anmor.SetBool(anmorIsWalkingTag, false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bullet")) // 當碰到子彈時
        {
            HpBar.SetHP(HpBar.HP - 10);
            if(HpBar.HP <= 0)
            {
                Destroy(HpBar.gameObject);
                Destroy(gameObject);
                return;
            }

            Vector2 pushForce = collision.collider.gameObject.transform.right;
            // 藉由子彈的移動方向計算要往哪被打飛
            pushForce.x *= xForceModify; // x和y上打飛的量可以不一樣
            pushForce.y *= yForceModify;
            Rgd.AddForce(pushForce);
        }
    }
}
