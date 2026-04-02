using System.Collections;
using UnityEngine;
// Bullet上的Script

[RequireComponent(typeof(AudioSource), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Bullet : MonoBehaviour, ICanKnockback
{
    private SpriteRenderer spRerr;
    public Sprite[] BulletSprites; // Bullet所有可能的Sprite，在Inspector中調整

    public GameObject BulletEffectPrefab;

    public AudioClip[] LaserSFX;
    private AudioSource audioSource;

    private Rigidbody2D rb;
    Rigidbody2D ICanKnockback.rb => rb;
    GameObject ICanKnockback.gameObject => gameObject;
    float ICanKnockback.collisionForceGiveRatio => 1.0f;
    float ICanKnockback.collisionForceKeepRatio => 0.0f;

    public float MoveSpeed;
    public float InitialDistance; // 在Bullet被創造時調整position的距離值

    public float Power;
    public ParticleSystem.MinMaxCurve DistacneToPowerCurve;
    private Vector2 initialPosition;

    public static float Damage = 5.0f;

    void Start()
    {
        // 隨機選擇一個Sprite
        spRerr = GetComponent<SpriteRenderer>();
        spRerr.sprite = BulletSprites[Random.Range(0, BulletSprites.Length)];

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = LaserSFX[Random.Range(0, LaserSFX.Length)];
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.Play();

        rb = GetComponent<Rigidbody2D>();

        // 計算Bullet的移動方向
        float rot = transform.rotation.eulerAngles.z;
        float dx = Mathf.Cos(rot * Mathf.Deg2Rad) * MoveSpeed;
        float dy = Mathf.Sin(rot * Mathf.Deg2Rad) * MoveSpeed;
        Vector2 dPos = new Vector2(dx, dy);
        transform.position += (Vector3)dPos * InitialDistance;
        initialPosition = transform.position;

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
    public void OnCollisionEnter2D(Collision2D collision)
    {

        GameObject other = collision.gameObject;
        
        if (other.layer == DefinedLayers.GroundLayer || other.layer == DefinedLayers.WallLayer)
        {
            SpawnEffects(3);
            ProperDestroy();
            return;
        }

        if(other.layer == DefinedLayers.EnemyLayer)
        {
            IKnockbackable otherKnockbackable;
            if(other.TryGetComponent<IKnockbackable>(out otherKnockbackable))
            {
                ((ICanKnockback)this).DoKnockback(
                    otherKnockbackable,
                    transform.right,
                    DistacneToPowerCurve.Evaluate(Vector2.Distance(transform.position, initialPosition)) * Power,
                    false
                );
            }

            AbstractEnemy enemy;
            if(!other.TryGetComponent<AbstractEnemy>(out enemy))
            {
                Debug.LogError($"GameObject {other.name} is having enemy layer but doesn't have AbstractEnemy Component.");
            }
            else
            {
                enemy.GetDamaged(Damage);
            }

            SpawnEffects(5);
            ProperDestroy();
            return;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Field"))
        {
            ProperDestroy();
            return;
        }
    }

    private bool _properDestroyCalled = false;
    private void ProperDestroy()
    {
        if(_properDestroyCalled){return;}
        transform.position = new Vector3(999f, 999f, 0f); // I fell bad about this.
        _properDestroyCalled = true;
        StartCoroutine(waitForAudioSourceBeforeDestroy());
    }

    private static readonly WaitForSeconds _waitQuarterSecond = new WaitForSeconds(0.25f);
    private IEnumerator waitForAudioSourceBeforeDestroy()
    {
        while(audioSource.isPlaying)
        {
            yield return _waitQuarterSecond;
        }
        Destroy(gameObject);
    }
}
