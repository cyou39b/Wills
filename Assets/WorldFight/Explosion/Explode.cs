using System.Collections;
using UnityEngine;

// 在wills1死亡時的爆炸

public class Explode : MonoBehaviour
{
    public static Vector2? ExplodePosition = null;
    public static bool Activated = false;

    public GameObject[] Explosions; // 所有可用的explosion prefab

    public int SpawnItsAddConstant;
    public int SpawnNumAtConstantDistance;
    public float SpawnCoolDown;
    
    public GameObject YouDiedGameObject;

    private WaitForSeconds wCoolDown;

    public void Start()
    {
        wCoolDown = new WaitForSeconds(SpawnCoolDown);
    }

    public void Update() {
        if(ExplodePosition == null){return;}

        Vector2 pos = (Vector2)ExplodePosition;
        ExplodePosition = null;
        Activated = true;
        StartExplosionAt(pos);
    }

    private static readonly WaitForSeconds waitOneSecond = new WaitForSeconds(1.0f);
    private static readonly WaitForFixedUpdate waitFixedUpdate = new WaitForFixedUpdate();
    // a fixed update is about 0.02s(50Hz);
    private static readonly WaitForSeconds waitThreeFixedUpdate = new WaitForSeconds(0.02f * 3);
    public void StartExplosionAt(Vector3 pos){
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
Spawn {0} Small Explosions.", 
            distance, 
            spawnIts, 
            spawnNum
        );

        // 向隨機方向生成spawnNum個explosion
        int halfSpawnNum=spawnNum/2;
        float dir = Random.Range(-Mathf.PI, Mathf.PI), rot = Mathf.PI*2.0f / halfSpawnNum;
        Vector3 moveVec = new Vector3(0.0f, 0.0f, 0.0f);
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

        DeathManager.StartDeath(0.0f, 2.0f);   
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
