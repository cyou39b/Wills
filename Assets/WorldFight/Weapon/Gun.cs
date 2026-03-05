using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour
{
    public GameObject BulletPrefab;

    public SpriteRenderer Sprerr;
    public Sprite[] GunFireSprites;
    public GameObject GunFireAnimation;
    public SpriteRenderer GunFireAnimationSpriteRenderer;
    
    private Jack jack;

    public float FireCoolDown;
    private float fireCoolDownTimer=0.0f;

    void Start()
    {
        GameObject parent = transform.parent.gameObject;
        if(!parent.TryGetComponent<Jack>(out jack))
        {
            Debug.LogError("Parent is Jack?");
        }
    }

    void Update()
    {
        // Do nothing if the game is paused
        if(Time.timeScale == 0.0f){return;}
        if(Explode.Activated){return;}

        // 計算mouse的角度
        Vector2 mousePixelPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mousePixelPosition);
        float mouseRot = Mathf.Atan2(
            mousePosition.y-transform.position.y,
            mousePosition.x-transform.position.x
        );

        // if(jack.dir == FacingDirection.Left) // EXPL: this sucks
        // {
        //     if(mouseRot < 0.0f) {mouseRot += Mathf.PI + Mathf.PI;}
        //     mouseRot = Mathf.Clamp(mouseRot, MathUtil.HalfPI + 0.001f, Mathf.PI + MathUtil.HalfPI - 0.001f);
        // }
        // else
        // {
        //     mouseRot = Mathf.Clamp(mouseRot, MathUtil.HalfNPI, MathUtil.HalfPI);
        // }

        if(mouseRot <= MathUtil.HalfPI && mouseRot >= MathUtil.HalfNPI)
        {
            Sprerr.flipY = false;
        }
        else
        {
            Sprerr.flipY = true;
        }

        transform.rotation = Quaternion.Euler(
            0.0f, 
            0.0f, 
            mouseRot * Mathf.Rad2Deg
        );

        fireCoolDownTimer -= Time.deltaTime; // 如果已經cooldown了
        if (fireCoolDownTimer <= 0.0f && Mouse.current.leftButton.wasPressedThisFrame)
        {
            fireCoolDownTimer = FireCoolDown;

            // 發射的小動畫
            GunFireAnimation.SetActive(true);
            GunFireAnimationSpriteRenderer.sprite = GunFireSprites[Random.Range(0, GunFireSprites.Length)];
            StartCoroutine(this.EndGunFireAnimation()); // 把動畫關掉的function

            // 生成一個子彈
            GameObject newObj = Instantiate(BulletPrefab, transform.position, transform.rotation);
            SpriteRenderer newObjSR;
            if(!newObj.TryGetComponent<SpriteRenderer>(out newObjSR))
            {
                Debug.LogWarning("Bullet doesn't have a Sprite Renderer");
            }
            else
            {
                if(colliderCount != 0)
                {
                    newObjSR.color = Color.clear;
                }
            }
        }

    }

    private static readonly WaitForSeconds EndGunFireAnimationWS = new WaitForSeconds(0.09f);
    public IEnumerator EndGunFireAnimation()
    {
        yield return EndGunFireAnimationWS;
        GunFireAnimation.SetActive(false);
    }

    private int colliderCount = 0;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            colliderCount++;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            colliderCount--;
        }
        
    }
}
