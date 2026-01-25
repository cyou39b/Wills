using UnityEngine;
// Bullet上的Script

public class Bullet : MonoBehaviour
{    
    public Sprite[] BulletSprites; // Bullet所有可能的Sprite，在Inspector中調整
    public SpriteRenderer BulletSpriteRenderer;
    
    public Rigidbody2D Rgd;
    public float MoveSpeed;
    public float InitialDistance; // 在Bullet被創造時調整position的距離值
    private float dx, dy;
    private Vector2 dPos;
    void Start()
    {
        // 隨機選擇一個Sprite
        BulletSpriteRenderer.sprite = BulletSprites[Random.Range(0, BulletSprites.Length)];

        // 計算Bullet的移動方向
        float rot = transform.rotation.eulerAngles.z;
        dx = Mathf.Cos(rot * Mathf.Deg2Rad) * MoveSpeed;
        dy = Mathf.Sin(rot * Mathf.Deg2Rad) * MoveSpeed;
        dPos = new Vector2(dx, dy);
        transform.position += new Vector3(dx * InitialDistance, dy*InitialDistance, 0.0f);
        
        // 設定這個Bullet的速度
        Rgd.linearVelocity = dPos;
    }
    void FixedUpdate()
    {
        // 如果這個Bullet跑到畫面外就把這個Bullet destroy
        if(transform.position.x < -15.0f || transform.position.x > 15.0f || transform.position.y > 6.0f)
        {
            Destroy(gameObject);
            return;
        }
    }

    // 在碰到另一個GameObject時會被call的function
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject colliderGameObject = collision.collider.gameObject;

        if (colliderGameObject.layer == GlobalVariables.GroundLayer)
        {
            Destroy(gameObject);
        }

        if(colliderGameObject.layer == GlobalVariables.EnemyLayer)
        {
            Destroy(gameObject);
        }
    }

}
