using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// 在wills1死亡時的爆炸

public class Explode : MonoBehaviour
{
    public GameObject[] Explosions; // 所有可用的explosion prefab
    public int SpawnIts;
    public int SpawnNum;
    public float SpawnCoolDown;
    private WaitForSeconds wCoolDown;

    public void Start()
    {
        wCoolDown = new WaitForSeconds(SpawnCoolDown);
        
        // 向隨機方向生成SpawnNum個explosion
        int i=0,halfSpawnNum=SpawnNum/2;
        float dir = Random.Range(-Mathf.PI, Mathf.PI), rot = Mathf.PI*2.0f / halfSpawnNum;
        Vector3 moveVec = new Vector3(0.0f, 0.0f, 0.0f);
        for (; i < halfSpawnNum; i++)
        {
            dir += rot;
            moveVec.x = Mathf.Cos(dir);
            moveVec.y = Mathf.Sin(dir);
            StartCoroutine(SpawnExplosions(transform.position, moveVec, SpawnIts));
        }
        for(; i < SpawnNum; i++)
        {
            dir = Random.Range(-Mathf.PI, Mathf.PI);
            moveVec.x = Mathf.Cos(dir);
            moveVec.y = Mathf.Sin(dir);
            StartCoroutine(SpawnExplosions(transform.position, moveVec, SpawnIts));
        }
        
    }

    // 一個由內往外生成explosion的IEnumerator，這個function會被掛在Coroutine上
    public IEnumerator SpawnExplosions(Vector3 position, Vector3 moveVec, int its)
    {
        StartCoroutine(SpawnAndCleanupExplosion(position));
        for(int i = 1; i < its; i++)
        {
            yield return wCoolDown;
            position += moveVec;
            StartCoroutine(SpawnAndCleanupExplosion(position));
        }
    }

    private static readonly WaitForSeconds waitOneSecond = new WaitForSeconds(1.0f);
    private static readonly WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

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
            yield return waitFrame;
        }
        Destroy(newObj);
    }
}
