using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class AbstractEnemy : MonoBehaviour, IKnockbackable, ICanKnockback
{
    protected Rigidbody2D rb;
    Rigidbody2D IKnockbackable.rb => rb; // I hate myself (and c#)
    Rigidbody2D ICanKnockback.rb => rb;
    GameObject IKnockbackable.gameObject => gameObject;
    GameObject ICanKnockback.gameObject => gameObject;
    protected new Collider2D collider;

    protected const float SlowDownRatio = 0.6f;
    protected const float SlowDownRatioHeadOn = 0.85f;
    public abstract float MoveSpeed{get;set;}
    protected HashSet<AbstractEnemy> slowedEnemy = new HashSet<AbstractEnemy>();
    public void SpeedChangeChainReaction(float changeRatio)
    {
        MoveSpeed *= changeRatio;
        foreach(AbstractEnemy next in slowedEnemy)
        {
            next.SpeedChangeChainReaction(changeRatio);
        }
    }

    public void UpdateStartOverlappingSpeed(AbstractEnemy other)
    {
        if(other.MoveSpeed > MoveSpeed || (other.MoveSpeed == MoveSpeed && other.GetInstanceID() > GetInstanceID()))
        {
            return;
        }

        slowedEnemy.Add(other);

        other.SpeedChangeChainReaction(SlowDownRatio);
    }

    public void UpdateEndOverlappingSpeed(AbstractEnemy other)
    {
        if(other.MoveSpeed > MoveSpeed || (other.MoveSpeed == MoveSpeed && other.GetInstanceID() > GetInstanceID()))
        {
            return;
        }

        slowedEnemy.Remove(other);
        other.SpeedChangeChainReaction(1.0f / SlowDownRatio);
    }

    protected GameObject renderingChildObject;
    protected SpriteRenderer SpRr;
    protected Material Mat;
    [NonSerialized] public Color mainColor = Color.green;
    protected Animator Anmor;

    public GameObject HPBarPrefab;
    public HPBar HpBar;

    public GameObject TrianglePrefab;
    protected Triangle triangle;

    protected GameObject player;
    protected Transform playerTrans;

    private Camera cam;

    protected virtual void Start()
    {
        cam = Camera.main;
        InitialPlayerInfoReference();
        InitializeRenderingGameObject();
        InitializeHpBar();
        InitializeTriangle();
        InitializePhysicComponents();
    }
    protected virtual void InitializePhysicComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }
    protected virtual void InitializeRenderingGameObject()
    {
        Transform rendererChildTrans = transform.Find("Renderer");
        if(rendererChildTrans == null)
        {
            Debug.Log("Abstract enemy should have a child gameobject for rendering called renderer");
        }
        renderingChildObject = rendererChildTrans.gameObject;
        if(!renderingChildObject.TryGetComponent<SpriteRenderer>(out SpRr))
        {
            Debug.LogError("AbstractEnemy doesn't have a Renderer child gameObject.");
        }
        if(!renderingChildObject.TryGetComponent<Animator>(out Anmor))
        {
            Debug.LogError("AbstractEnemy doesn't have a Renderer child gameObject.");
        }

        Mat = SpRr.material;
        if(Mat == null)
        {
            Debug.LogError("SpriteRenderer.material is null");
        }
    }
    protected virtual void InitializeTriangle()
    {
        GameObject triangleGameObject = Instantiate(TrianglePrefab, transform.position, transform.rotation);
        if(!triangleGameObject.TryGetComponent<Triangle>(out this.triangle))
        {
            Debug.LogError("Triangle doesn't have triangle component.");
        }
        triangle.Initialize(this, mainColor);
        
        #if UNITY_EDITOR
        triangle.name = string.Format("{0} - Triangle", gameObject.name);
        #endif
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
    public abstract (float ,float , Vector3)HpBarData{get;}
    protected virtual void InitializeHpBar()
    {
        GameObject hpBarGameObject = Instantiate(HPBarPrefab, transform.position, transform.rotation);
        if(!hpBarGameObject.TryGetComponent<HPBar>(out HpBar))
        {
            Debug.LogErrorFormat("Failed to get HPBar component from gameObject created for GameObject named {}.", gameObject.name);
        }
        else
        {
            (float maxHp, float hp, Vector3 offset) = HpBarData;

            HpBar.Followee = gameObject;
            HpBar.Offset = offset;
            HpBar.OnHpLE0 = OnHPLE0;
            HpBar.MaxHP = maxHp;
            HpBar.HP = hp;

            #if UNITY_EDITOR
            HpBar.name = string.Format("{0} - HPBar", gameObject.name);
            #endif
        }
    }
    public virtual void GetDamaged(float amount)
    {
        if(amount <= 0.0f)
        {
            Debug.LogError("Invalid Argument");
        }

        HpBar.HP -= amount;
        StartCoroutine(OnDamagedBlink());
    }
    private static readonly WaitForSeconds blinkTimeSpan = new WaitForSeconds(1.0f / 60.0f * 3.0f);
    public virtual IEnumerator OnDamagedBlink()
    {
        // Unlike animator, material doesn't have SetBool
        // use SetInt or SetFloat instead
        Mat.SetInt("_Blink", 1);
        Color originalColor = triangle.wills1Color;
        Debug.Log(originalColor);
        triangle.wills1Color = new Color(1.0f, 1.0f, 1.0f, originalColor.a);

        yield return blinkTimeSpan;

        Mat.SetInt("_Blink", 0);
        triangle.wills1Color = new Color(
            originalColor.r,
            originalColor.g,
            originalColor.b,
            triangle.wills1Color.a
        );
    }
    protected virtual void OnHPLE0()
    {
        Explode.ExplodePosition = transform.position;
        Destroy(triangle.gameObject);
        Destroy(HpBar.gameObject);
        Destroy(gameObject);
    }
    protected virtual void OnOutOfField()
    {
        Debug.Log("Enemy out of field");

        // because that stupid OnTriggerExit2D was called when the `field`
        // GameObject got destroyed while loading another scene/ quitting application
        // so I have to do this null check every time.
        if(triangle != null) {Destroy(triangle.gameObject);}
        if(HpBar != null) {Destroy(HpBar.gameObject);}
        if(gameObject != null) 
        {
            RbCameraMovement.Enemys.Remove(transform);
            Destroy(gameObject);
        }
    }

    // I hate this implementation. Previous one with coroutine is prettier.
    // But the problem is... you know coroutine ¯\_(ツ)_/¯ 
    // Hope I find a better one soon.
    public enum Intent
    {
        Idle,
        RandomlyRoam,
        ChasePlayer,
        Attack,
        PrepJump,
        Jump,
        WalkOffEdge,
        WaitUntilStill,
        WaitUntilGround,
        GetKnockbacked,
        EnumCount
    }
    public virtual Intent intent{get; set;}
    protected void SetIntent(Intent to, AIFacingDirection direction, bool forceOverwrite = false, Intent expectFrom = Intent.EnumCount)
    {
        // if force overwrite or the caller knows what it's doing
        if(forceOverwrite || expectFrom == intent)
        {
            // change intent
            Debug.Log($"{name} intent change: {intent} => {to}");
            intent = to;
            AIFacingDirection = direction;
        }
        else
        {
            // set intent to the one with higher priority
            if((int)to > (int)intent)
            {
                Debug.Log($"{name} intent change: {intent} => {to}");
                intent=to;
                AIFacingDirection = direction;
            }
        }
        // yeah, at this point I'm pretty sure that I hate this implementaion.
    }
    protected abstract void MainProcessIntent();
    protected bool isIntentPassive;
    protected AIFacingDirection AIFacingDirection;
    protected virtual FacingDirection CurrentRealFacingDirection{get;set;}

    public bool AI = true;
    protected virtual void FixedUpdate()
    {
        MainProcessIntent();
    }

    public abstract void GetKnockbacked(Vector2 direction, float power, bool stun, Vector2 forcePosition);

    protected HashSet<IKnockbackable> knockbackablesInContact = new HashSet<IKnockbackable>();

    public float collisionForceGiveRatio = 0.75f; // 0.9f and 0.3f is tested value.
    public float collisionForceKeepRatio = 0.4f;

    float ICanKnockback.collisionForceGiveRatio => collisionForceGiveRatio;
    float ICanKnockback.collisionForceKeepRatio => collisionForceKeepRatio;
    public void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        IKnockbackable knockbackable;
        if(other.TryGetComponent<IKnockbackable>(out knockbackable))
        {
            knockbackablesInContact.Add(knockbackable);
            if(intent == Intent.WaitUntilStill)
            {
                ((ICanKnockback)this).DoKnockback(
                    knockbackable,
                    rb.linearVelocity.normalized,
                    rb.linearVelocity.magnitude * rb.mass / Time.fixedDeltaTime * collisionForceGiveRatio,
                    false
                );
            }
        }

        AbstractEnemy abstractEnemy;
        if(other.TryGetComponent<AbstractEnemy>(out abstractEnemy))
        {
            UpdateStartOverlappingSpeed(abstractEnemy);
        }
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        IKnockbackable knockbackable;
        if(other.TryGetComponent<IKnockbackable>(out knockbackable))
        {
            knockbackablesInContact.Remove(knockbackable);
        }

        AbstractEnemy abstractEnemy;
        if(other.TryGetComponent<AbstractEnemy>(out abstractEnemy))
        {
            UpdateEndOverlappingSpeed(abstractEnemy);
        }
    }
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Field"))
        {
            OnOutOfField();
            return;
        }
    }

    public void InsertAndRemoveFromEnemyList()
    {
        if(RbCameraMovement.Enemys.ContainsKey(transform))
        {
            if(camCoroutine != null) {StopCoroutine(camCoroutine);}
            camCoroutine = StartCoroutine(InsertAndRemoveFromEnemyListIt());
            return;
        }

        Vector3 viewPort = cam.WorldToViewportPoint(transform.position);
        bool onCamera = viewPort.z > 0.0f && 
                        viewPort.x >= 0.0f && viewPort.x <= 1.0f && 
                        viewPort.y >= 0.0f && viewPort.y <= 1.0f;
        if(onCamera)
        {
            camCoroutine = StartCoroutine(InsertAndRemoveFromEnemyListIt());
        }
    }

    Coroutine camCoroutine = null;
    private static readonly float maxPanTime = 0.95f;
    private IEnumerator InsertAndRemoveFromEnemyListIt()
    {
        RbCameraMovement.Enemys[transform] = rb;

        float startTime = Time.time;
        while(true)
        {
            yield return new WaitForFixedUpdate();
            if(intent == Intent.GetKnockbacked)
            {
                startTime = Time.time;
            }
            else if(intent != Intent.WaitUntilGround && intent != Intent.WaitUntilStill) {break;}

            if(Time.time - startTime >= maxPanTime) {break;}
        }

        RbCameraMovement.Enemys.Remove(transform);
    }
}