using System.Collections;
using UnityEngine;
// Bullet上的Script

public class Bullet : MonoBehaviour
{    
    private SpriteRenderer spRerr;
    public Sprite[] BulletSprites; // Bullet所有可能的Sprite，在Inspector中調整

    public GameObject BulletEffectPrefab;

    private Rigidbody2D rb;

    public float MoveSpeed;
    public float InitialDistance; // 在Bullet被創造時調整position的距離值
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 隨機選擇一個Sprite
        spRerr = GetComponent<SpriteRenderer>();
        spRerr.sprite = BulletSprites[Random.Range(0, BulletSprites.Length)];

        // 計算Bullet的移動方向
        float rot = transform.rotation.eulerAngles.z;
        float dx = Mathf.Cos(rot * Mathf.Deg2Rad) * MoveSpeed;
        float dy = Mathf.Sin(rot * Mathf.Deg2Rad) * MoveSpeed;
        Vector2 dPos = new Vector2(dx, dy);
        transform.position += new Vector3(dx * InitialDistance, dy*InitialDistance, 0.0f);
        
        // 設定這個Bullet的速度
        rb.linearVelocity = dPos;
    }

    private bool spawnEffectActivated = false;
    public void SpawnEffects(int num)
    {
        if(spawnEffectActivated){return;}
        spawnEffectActivated = true;
        for(int i = 0; i < num; i++)
        {
            Vector2 randPoint = MathUtil.RandomPointInCircle(
                transform.position,
                0.4f
            );

            Instantiate(
                BulletEffectPrefab,
                MathUtil.Vector2ToVecotr3(randPoint, transform.position.z),
                transform.rotation
            );
        }
    }

    // 在碰到另一個GameObject時會被call的function
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject colliderGameObject = collision.collider.gameObject;

        if (colliderGameObject.layer == GlobalVariables.GroundLayer || colliderGameObject.layer == GlobalVariables.WallLayer)
        {
            SpawnEffects(3);
            Destroy(gameObject);
            return;
        }

        if(colliderGameObject.layer == GlobalVariables.EnemyLayer)
        {
            SpawnEffects(3);
            Destroy(gameObject);
            return;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Field"))
        {
            SpawnEffects(3);
            Destroy(gameObject);
            return;
        }
    }
}
