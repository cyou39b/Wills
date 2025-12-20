using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    
    public Sprite[] BulletSprites;
    public SpriteRenderer BulletSpriteRenderer;
    
    public Rigidbody2D Rgd;
    public float MoveSpeed;
    private float dx, dy;
    private Vector2 dPos;
    void Start()
    {
        BulletSpriteRenderer.sprite = BulletSprites[Random.Range(0, BulletSprites.Length)];
        float rot = Random.Range(-Mathf.PI, Mathf.PI);
        dx = Mathf.Cos(rot) * MoveSpeed;
        dy = Mathf.Sin(rot) * MoveSpeed;
        dPos = new Vector2(dx, dy);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rot * Mathf.Rad2Deg);
    }
    void Update()
    {
        Rgd.linearVelocity = dPos;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

}
