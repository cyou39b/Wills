using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public GameObject BulletPrefab;
    public Sprite[] GunFireSprites;
    public SpriteRenderer GunFireAnimationSpriteRenderer;
    public GameObject GunFireAnimation;
    public SpriteRenderer Sprerr;
    public float FireCoolDown;
    private float fireCoolDownTimer=0.0f;
    void Update()
    {
        // Do nothing if the game is paused
        if(Time.timeScale == 0.0f){return;}
        
        // 計算mouse的角度
        Vector2 mousePixelPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mousePixelPosition);
        float mouseRot = Mathf.Atan2(
            mousePosition.y-transform.position.y,
            mousePosition.x-transform.position.x
        );

        if(mouseRot <= GlobalVariables.HalfPI && mouseRot >= GlobalVariables.HalfNPI)
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
            Instantiate(BulletPrefab, transform.position, transform.rotation);
        }

    }

    private static readonly WaitForSeconds EndGunFireAnimationWS = new WaitForSeconds(0.09f);
    public IEnumerator EndGunFireAnimation()
    {
        yield return EndGunFireAnimationWS;
        GunFireAnimation.SetActive(false);
    }
}
