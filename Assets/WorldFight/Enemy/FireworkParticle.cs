using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(ParticleSystem))]
public class FireworkParticle : MonoBehaviour
{
    private ParticleSystem partSys;
    private Rigidbody2D rb;
    private Color color;
    private Vector3 endPos = Vector3.zero;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector3(0.0f, 7.5f, 0.0f);
        
        partSys = GetComponent<ParticleSystem>();
        AfterStartAndInit();
    }

    public void Initialize(Color wills1Color, Vector3 wills1Pos)
    {
        color = wills1Color;
        endPos = wills1Pos;
        wills1Pos.y = -10.0f;
        transform.position = wills1Pos;

        AfterStartAndInit();
    }

    bool startOrInit = false;
    void AfterStartAndInit()
    {
        if (!startOrInit)
        {
            startOrInit = true;
            return;
        }

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]{new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f)},
            new GradientAlphaKey[]{new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 0.8f), new GradientAlphaKey(0.0f, 1.0f)}
        );

        // Who the fuck thinks it's a good idea to make structs and classes 
        // that basically have no difference 
        // until you got bugged because you didn't realize that 
        // you're editing a fucking reference instead of a locally stored struct.
        ParticleSystem.ColorOverLifetimeModule colorModule = partSys.colorOverLifetime;
        // And also get and set function sucks.
        // At first they seems to be a pretty elegant💅 solution to readability some issue, 
        // but end up being super implicit and trap you because you didn't 
        // check if that field is real or is just some wierd get&set function
        // or trap you because you didn't decide to dig into some decompiled 
        // files to see what actually going on under that "convenient" get&set function.
        colorModule.color = gradient;
    }

    void FixedUpdate()
    {
        if (!partSys.IsAlive())
        {
            Destroy(gameObject);
        }

        if(transform.position.y >= endPos.y)
        {
            ParticleSystem.EmissionModule emissionModule = partSys.emission;
            emissionModule.enabled = false;
            rb.linearVelocity = Vector3.zero;
        }
    }
}
