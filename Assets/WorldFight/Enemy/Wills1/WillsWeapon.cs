using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class WillsWeapon : MonoBehaviour
{
    public GameObject BulletPrefab;

    [System.NonSerialized] public SpriteRenderer spriteRenderer;
    public Sprite[] GunFireSprites;
    private GameObject GunFireAnimation;
    public SpriteRenderer GunFireAnimationSpriteRenderer;

    void Awake()
    {
        GunFireAnimation = GunFireAnimationSpriteRenderer.gameObject;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetRotation(Vector3 dir)
    {
        dir.z = 0.0f;
        float rot = Mathf.Atan2(dir.y, dir.x);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rot * Mathf.Rad2Deg);
        if(rot <= MathUtil.HalfPI && rot >= MathUtil.HalfNPI)
        {
            spriteRenderer.flipY = false;
        }
        else
        {
            spriteRenderer.flipY = true;
        }
    }

    public void Fire()
    {
        GunFireAnimation.SetActive(true);
        GunFireAnimationSpriteRenderer.sprite = GunFireSprites[Random.Range(0, GunFireSprites.Length)];
        StartCoroutine(this.EndGunFireAnimation()); // 把動畫關掉的function

        Instantiate(BulletPrefab, transform.position, transform.rotation);
    }

    private static readonly WaitForSeconds EndGunFireAnimationWS = new WaitForSeconds(0.09f);
    public IEnumerator EndGunFireAnimation()
    {
        yield return EndGunFireAnimationWS;
        GunFireAnimation.SetActive(false);
    }
}
