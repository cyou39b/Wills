using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    
    public Sprite[] BulletSprites;
    public SpriteRenderer BulletSpriteRenderer;
    
    public Rigidbody2D Rgd;
    public float MoveSpeed;
    public float InitialDistance;
    private float dx, dy;
    private Vector2 dPos;
    void Start()
    {
        BulletSpriteRenderer.sprite = BulletSprites[Random.Range(0, BulletSprites.Length)];
        float rot = transform.rotation.z;
        dx = Mathf.Cos(rot) * MoveSpeed;
        dy = Mathf.Sin(rot) * MoveSpeed;
        dPos = new Vector2(dx, dy);
        transform.position += new Vector3(dx * InitialDistance, dy*InitialDistance, 0.0f);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rot * Mathf.Rad2Deg);
    }
    void Update()
    {
        Rgd.linearVelocity = dPos;
        if(transform.position.x < -15.0f || transform.position.x > 15.0f || transform.position.y > 6.0f)
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Wills"))
        {
            Destroy(gameObject);
        }
    }

}
