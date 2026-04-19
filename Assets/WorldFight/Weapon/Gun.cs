using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
public class Gun : MonoBehaviour
{
    public GameObject BulletPrefab;

    private SpriteRenderer Sprerr;
    public Sprite[] GunFireSprites;
    private GameObject GunFireAnimation;
    public SpriteRenderer GunFireAnimationSpriteRenderer;
    
    private Camera cam;

    public float FireCoolDown;
    private float fireCoolDownTimer=0.0f;

    public float AttackFreezeTime;
    [NonSerialized] public float attackFreezeTimer;
    [NonSerialized] public bool facingLeft;

    void Start()
    {
        cam = Camera.main;
        Sprerr = GetComponent<SpriteRenderer>();
        GunFireAnimation = GunFireAnimationSpriteRenderer.gameObject;
    }

    void Update()
    {
        // Do nothing if the game is paused
        if(Time.timeScale == 0.0f){return;}
        if(Explode.Activated){return;}

        // 計算mouse的角度
        Vector2 mousePixelPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = cam.ScreenToWorldPoint(mousePixelPosition);
        float mouseRot = Mathf.Atan2(
            mousePosition.y-transform.position.y,
            mousePosition.x-transform.position.x
        );

        if(mouseRot <= MathUtil.HalfPI && mouseRot >= MathUtil.HalfNPI)
        {
            Sprerr.flipY = false;
            facingLeft = false;
        }
        else
        {
            Sprerr.flipY = true;
            facingLeft = true;
        }

        transform.rotation = Quaternion.Euler(
            0.0f, 
            0.0f, 
            mouseRot * Mathf.Rad2Deg
        );

        fireCoolDownTimer -= Time.deltaTime; // 如果已經cooldown了
        attackFreezeTimer -= Time.deltaTime;
    }

    public void TryFire()
    {
        if (fireCoolDownTimer <= 0.0f)
        {
            fireCoolDownTimer = FireCoolDown;
            attackFreezeTimer = AttackFreezeTime;

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
