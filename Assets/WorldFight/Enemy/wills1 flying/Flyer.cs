using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Flyer : AbstractEnemy
{
    public Color[] FaceColors;
    public Color[] FaceOutlineColors;
    public Color[] EyesColors;
    public Color[] WingBColors;
    public Color[] WingGColors;
    public Color[] WingYColors;

    public float xForceModify, yForceModify;

    protected override void Start()
    {
        base.Start();

        Material mat = SpRr.material;
        if(mat == null)
        {
            Debug.LogError("material is null.");
        }

        int idx = Random.Range(0, FaceColors.Length);
        mainColor = FaceColors[idx];
        mat.SetColor("_Eyes_Color", EyesColors[idx]);
        mat.SetColor("_Face_Color", FaceColors[idx]);
        mat.SetColor("_Face_Outline_Color", FaceOutlineColors[idx]);
        mat.SetColor("_WingB_Color", WingBColors[idx]);
        mat.SetColor("_WingG_Color", WingGColors[idx]);
        mat.SetColor("_WingY_Color", WingYColors[idx]);
    }

    private void FixedUpdate()
    {
        rb.AddForceY(1.0f);
    }

    protected override (float, float, Vector3?) HpBarData() 
        => (40.0f, 40.0f, new Vector3(0.0f, 1.15f, 0.0f));

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
