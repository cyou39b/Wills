using System.Collections;
using UnityEngine;

// 在wills1死亡時的爆炸

[RequireComponent(typeof(AudioSource))]
public class Explode : MonoBehaviour
{
    public static Vector3? ExplodePosition = null;
    public static bool Activated = false;

    public GameObject[] Explosions; // 所有可用的explosion prefab
    
    private AudioSource audioSrc;
    public AudioClip[] Clips;

    public int SpawnItsAddConstant;
    public int SpawnNumAtConstantDistance;
    public float SpawnCoolDown;
    
    private WaitForSeconds wCoolDown;

    public void Start()
    {
        wCoolDown = new WaitForSeconds(SpawnCoolDown);
        audioSrc = GetComponent<AudioSource>();
    }

    public void Update() {
        if(ExplodePosition == null || Activated){return;}
        Vector3 pos = (Vector3)ExplodePosition;
        ExplodePosition = null;
        Activated = true;

        Camera cam = Camera.main;
        Vector3 viewPortPoint = cam.WorldToViewportPoint(pos) - new Vector3(0.5f, 0.5f, 0.0f);
        Vector3.ClampMagnitude(viewPortPoint, 1.0f);
        viewPortPoint += new Vector3(0.5f, 0.5f, 0.0f);
        pos = cam.ViewportToWorldPoint(viewPortPoint);

        RbCameraMovement rbCameraMovement;
        if(cam.TryGetComponent<RbCameraMovement>(out rbCameraMovement))
        {
            rbCameraMovement.Shake(0.8f, 10.0f);
        }

        StartCoroutine(StartExplosionAt(pos));
        audioSrc.clip = Clips[Random.Range(0, Clips.Length)];
        audioSrc.Play();
    }

    private static readonly WaitForSeconds waitOneSecond = new WaitForSeconds(1.0f);
    private static readonly WaitForFixedUpdate waitFixedUpdate = new WaitForFixedUpdate();
    // a fixed update is about 0.02s(50Hz);
    public IEnumerator StartExplosionAt(Vector3 pos){
        pos.z = -1.0f; // make the layer of explosions to be closer

        Vector2 playerPos = transform.position;
        float distance = Vector2.Distance(playerPos, pos);

        int spawnIts = SpawnItsAddConstant + (int)distance;

        int spawnNum = (int)(
            SpawnNumAtConstantDistance*
            ((distance+SpawnItsAddConstant)/SpawnItsAddConstant)*
            ((distance+SpawnItsAddConstant)/SpawnItsAddConstant)
        );

        Debug.LogFormat(@"
Distance between Player and exploding Wills is {0}
Spawn Its is {1}
Spawn {2} Small Explosions.", 
            distance, 
            spawnIts, 
            spawnNum
        );

        // 向隨機方向生成spawnNum個explosion
        int halfSpawnNum=spawnNum/2;
        float dir = Random.Range(-Mathf.PI, Mathf.PI), rot = Mathf.PI*2.0f / halfSpawnNum;
        Vector3 moveVec = Vector3.zero;
        for (int i=0; i < halfSpawnNum; i++)
        {
            dir += rot;
            moveVec.x = Mathf.Cos(dir);
            moveVec.y = Mathf.Sin(dir);
            StartCoroutine(SpawnExplosionLine(pos, moveVec, spawnIts));
        }
        for(int i=halfSpawnNum; i < spawnNum; i++)
        {
            dir = Random.Range(-Mathf.PI, Mathf.PI);
            moveVec.x = Mathf.Cos(dir);
            moveVec.y = Mathf.Sin(dir);
            StartCoroutine(SpawnExplosionLine(pos, moveVec, spawnIts));
        }

        yield return new WaitForSeconds(0.04f * spawnIts);
        DeathManager.StartDeath(0.0f, 1.0f);   
    }

    // 一個由內往外生成explosion的IEnumerator，這個function會被掛在Coroutine上
    public IEnumerator SpawnExplosionLine(Vector3 center, Vector3 moveVec, int its)
    {
        Vector3 position = center;
        float[] stepDistances = new float [its];
        float totalDistance = its * 1.3f;
        for(int i = its; i > 0; i--)
        {
            float avgDistance = totalDistance/i;
            float thisStepDistance = Random.Range(0.0f, avgDistance / 2);
            totalDistance -= thisStepDistance;
            stepDistances[i-1] = thisStepDistance;
        }

        for(int i = 1; i < its; i++)
        {
            yield return wCoolDown;
            position += moveVec * stepDistances[i];
            StartCoroutine(SpawnAndCleanupExplosion(position));
            StartCoroutine(SpawnAndCleanupExplosion(
                MathUtil.Vector2ToVecotr3(MathUtil.RandomPointInCircle(center, i), position.z)
            ));
        }
    }


    // 生成、調整動畫和移除explosion的IEnumrator，這個function會被掛在Coroutine上
    public IEnumerator SpawnAndCleanupExplosion(Vector3 position)
    {
        GameObject newObj = Instantiate(
            Explosions[Random.Range(0,Explosions.Length)],
            position,
            Quaternion.Euler(0.0f, 0.0f, Random.Range(-180.0f, 180.0f))
        );
        yield return waitOneSecond; // wait for animation

        SpriteRenderer sprr;
        if(!newObj.TryGetComponent<SpriteRenderer>(out sprr))
        {
            Destroy(newObj);
            Debug.LogError("Explosion GameObject doesn't have SpriteRenderer Component.");
            yield break; // yield break end this coroutine
        }

        // 讓explosion的gameObject在動畫結束後fade out
        Color spriteColor = sprr.color;
        for(int i = 0; i < 20; i++)
        {
            spriteColor.a *= 0.85f;
            sprr.color = spriteColor;
            yield return waitFixedUpdate;
        }
        Destroy(newObj);
    }
}
