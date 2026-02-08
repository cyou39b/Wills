using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class AbstractEnemy : MonoBehaviour
{
    protected Rigidbody2D rb;

    protected GameObject renderingChildObject;
    protected SpriteRenderer SpRr;
    protected Color mainColor = Color.green;

    public GameObject HPBarPrefab;
    protected HPBar HpBar;

    public GameObject TrianglePrefab;
    protected Triangle triangle;

    protected GameObject player;
    protected Transform playerTrans;

    protected virtual void Start()
    {
        InitialPlayerInfoReference();
        InitializeRenderingGameObject();
        InitializeHpBar();
        InitializeTriangle();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void InitializeRenderingGameObject()
    {
        foreach(Transform childTransform in transform)
        {
            GameObject child = childTransform.gameObject;
            if(child.name.Equals("Renderer"))
            {
                renderingChildObject = child;
                if(!renderingChildObject.TryGetComponent<SpriteRenderer>(out SpRr))
                {
                    Debug.LogError("AbstractEnemy doesn't have a Renderer child gameObject.");
                }
                break;
            }
        }
    }

    protected virtual void InitializeTriangle()
    {
        GameObject triangleGameObject = Instantiate(TrianglePrefab, transform.position, transform.rotation);
        if(!triangleGameObject.TryGetComponent<Triangle>(out this.triangle))
        {
            Debug.LogError("Triangle doesn't have triangle component.");
        }
        triangle.Initialize(gameObject, mainColor);
    }

    protected virtual void InitialPlayerInfoReference()
    {
        player = GameObject.FindWithTag("Player");
        if(player == null)
        {
            Debug.LogError("Can't find player in current scene.");
        }
        playerTrans = player.transform;
    }

    protected abstract (float ,float , Vector3?)HpBarData();
    protected virtual void InitializeHpBar()
    {
        GameObject hpBarGameObject = Instantiate(HPBarPrefab, transform.position, transform.rotation);
        if(!hpBarGameObject.TryGetComponent<HPBar>(out HpBar))
        {
            Debug.LogErrorFormat("Failed to get HPBar component from gameObject created for GameObject named {}.", gameObject.name);
        }
        else
        {
            (float maxHp, float hp, Vector3? offset) = HpBarData();

            HpBar.Followee = gameObject;
            HpBar.Offset = (offset == null)?Vector3.zero:offset.Value;
            HpBar.OnHpLE0 = OnHPLE0;
            HpBar.MaxHP = maxHp;
            HpBar.HP = hp;

            #if UNITY_EDITOR
            HpBar.name = string.Format("{0} - HPBar", gameObject.name);
            #endif
        }
    }

    protected virtual void OnOutOfField()
    {
        Debug.Log("Enemy out of field");

        // because that stupid OnTriggerExit2D was called when the `field`
        // GameObject got destroyed while loading another scene/ quitting application
        // so I have to do this null check every time.
        if(triangle != null) {Destroy(triangle.gameObject);}
        if(HpBar != null) {Destroy(HpBar.gameObject);}
        if(gameObject != null) {Destroy(gameObject);}
    }

    protected virtual void OnHPLE0()
    {
        Explode.ExplodePosition = transform.position;
        Destroy(triangle.gameObject);
        Destroy(HpBar.gameObject);
        Destroy(gameObject);
    }

    protected abstract void OnCollisionEnter2D(Collision2D collision);
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Field"))
        {
            OnOutOfField();
            return;
        }
    }
}