using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public GameObject BulletPrefab;
    public Sprite[] GunFireSprites;
    public SpriteRenderer GunFireAnimationSpriteRenderer;
    public GameObject GunFireAnimation;
    public float FireCoolDown;
    private float fireCoolDownTimer=0.0f;
    void Update()
    {
        // Do nothing if the game is stopped
        if(Time.timeScale == 0.0f){return;}
        
        fireCoolDownTimer -= Time.deltaTime;
        if (fireCoolDownTimer <= 0.0f && Mouse.current.leftButton.wasPressedThisFrame)
        {
            fireCoolDownTimer = FireCoolDown;
            GunFireAnimation.SetActive(true);
            GunFireAnimationSpriteRenderer.sprite = GunFireSprites[Random.Range(0, GunFireSprites.Length)];
            Instantiate(BulletPrefab, transform.position, transform.rotation);
            StartCoroutine(this.EndGunFireAnimation());
        }

        Vector2 mousePixelPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mousePixelPosition);
        transform.rotation = Quaternion.Euler(
            0.0f, 
            0.0f, 
            Mathf.Atan2(
                mousePosition.y-transform.position.y,
                mousePosition.x-transform.position.x
            ) * Mathf.Rad2Deg
        );
    }

    private static readonly WaitForSeconds EndGunFireAnimationWS = new WaitForSeconds(0.09f);
    public IEnumerator EndGunFireAnimation()
    {
        yield return EndGunFireAnimationWS;
        GunFireAnimation.SetActive(false);
    }
}
