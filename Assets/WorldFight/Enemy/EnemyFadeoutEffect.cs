using UnityEngine;

public class EnemyFadeoutEffect : MonoBehaviour
{
    void Awake() {enabled = false;}

    private SpriteRenderer[] spriteRenderers = new SpriteRenderer[0];
    public float FadeoutTime;
    private float FadeoutTimer = 0.0f;
    public float RbMaxVelocityMagnitude;

    public void Initialize(SpriteRenderer[] enemySpriteRenderer, Rigidbody2D rb)
    {
        enabled = true;
        rb.gravityScale = 0.0f;
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, RbMaxVelocityMagnitude);
        spriteRenderers = enemySpriteRenderer;
    }

    public void Update()
    {
        FadeoutTimer += Time.deltaTime;

        if(FadeoutTimer>FadeoutTime)
        {
            if(FadeoutTimer>FadeoutTime+0.3f)
            {
                RbCameraMovement.Enemys.Remove(transform);
                Destroy(gameObject);
                return;
            }
            return;
        }

        foreach(SpriteRenderer spriteRenderer in spriteRenderers)
        {
            Color newColor = spriteRenderer.color; newColor.a = 1.0f - FadeoutTimer / FadeoutTime;
            spriteRenderer.color = newColor;
        }
    }
}
