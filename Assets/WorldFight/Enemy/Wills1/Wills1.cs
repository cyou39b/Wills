using UnityEngine;

public class Wills : AbstractEnemy
{
    public Color[] BodyColors;
    public Color[] EyeColors;

    public Animator Anmor;
    private static readonly string anmorIsWalkingTag="isWalking";
    private bool _walking = false;
    private bool walking 
    {
        get => _walking;
        set
        {
            if(_walking != value)
            {
                // Anmor.SetBool(anmorIsWalkingTag, true); <- 把wills1的動畫調成正在移動
                // Anmor.SetBool(anmorIsWalkingTag, false); <- 把動畫調成站著不動
                Anmor.SetBool(anmorIsWalkingTag, value);
                _walking = value;
            }
        }
    } 
    
    public float xForceModify;
    public float yForceModify;
    public float MoveSpeed;
    private FacingDirection dir = FacingDirection.None;

    public GameObject FireworkPrefab;

    protected override void Start()
    {
        base.Start();

        Material mat = SpRr.material;
        if(mat == null)
        {
            Debug.LogError("SpriteRenderer.material is null");
        }
        if(BodyColors.Length != EyeColors.Length)
        {
            Debug.LogError("BodyColors.Length != EyeColors.Length");
        }
        if(BodyColors.Length == 0)
        {
            Debug.LogError("BodyColors.Length == 0");
        }

        int idx = Random.Range(0, BodyColors.Length);
        mat.SetColor("_TargetColor", BodyColors[idx]);
        mainColor = BodyColors[idx];
        mat.SetColor("_EyeColor", EyeColors[idx]);
    }

    protected override (float, float, Vector3?) HpBarData() 
        => (40.0f, 40.0f, new Vector3(0.0f, 1.4f, 0.0f));

    void FixedUpdate()
    {
        if(Mathf.Abs(transform.position.x - playerTrans.position.x) <= 0.05f) {
            dir = FacingDirection.None;
        } else if(transform.position.x > playerTrans.position.x) {
            dir = FacingDirection.Left;
        } else {
            dir = FacingDirection.Right;
        }

        switch(dir)
        {
            case FacingDirection.Left:
                rb.linearVelocityX = -MoveSpeed;
                break;
            case FacingDirection.Right:
                rb.linearVelocityX = MoveSpeed;
                break;
            case FacingDirection.None:
                rb.linearVelocityX = 0.0f;
                break;
        }

        HpBar.HP += 0.02f;
    }

    public void Update()
    {
        switch(dir)
        {
            case FacingDirection.Left:
                SpRr.flipX = true;
                walking = true;
                break;
            case FacingDirection.Right:
                SpRr.flipX = false;
                walking = true;
                break;
            case FacingDirection.None:
                walking = false;
                break;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bullet")) // 當碰到子彈時
        {
            HpBar.HP -= 10.0f;

            Vector2 pushForce = collision.collider.gameObject.transform.right;
            // 藉由子彈的移動方向計算要往哪被打飛
            pushForce.x *= xForceModify; // x和y上打飛的量可以不一樣
            pushForce.y *= yForceModify;
            rb.AddForce(pushForce);
        }
    }
}
