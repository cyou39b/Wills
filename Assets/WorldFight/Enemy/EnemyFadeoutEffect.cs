using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class EnemyFadeoutEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        RbCameraMovement.Enemys.Add(transform, rb);
    }

    public void Initialize(in SpriteRenderer enemySpriteRenderer, Vector3 rendererPositionOffset, in Rigidbody2D enemyRb, Vector3 scale)
    {
        spriteRenderer.material = enemySpriteRenderer.material;
        spriteRenderer.sprite = enemySpriteRenderer.sprite;
        spriteRenderer.color = enemySpriteRenderer.color;

        rb.linearVelocity = enemyRb.linearVelocity;

        transform.position += rendererPositionOffset;
        transform.localScale = scale;
    }

    public float FadeoutTime;
    private float FadeoutTimer = 0.0f;
    public void Update()
    {
        FadeoutTimer += Time.deltaTime;

        if(FadeoutTimer>FadeoutTime)
        {
            rb.linearVelocity = Vector2.zero;
            if(FadeoutTimer>FadeoutTime+0.3f)
            {
                RbCameraMovement.Enemys.Remove(transform);
                Destroy(gameObject);
                return;
            }
            return;
        }

        Color newColor = spriteRenderer.color; newColor.a = 1.0f - FadeoutTimer / FadeoutTime;
        spriteRenderer.color = newColor;
    }
}
