using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Explode : MonoBehaviour
{
    public GameObject[] Explosions;
    public int SpawnIts;
    public int SpawnNum;
    public float SpawnCoolDown;
    private WaitForSeconds wCoolDown;

    void Start()
    {
        wCoolDown = new WaitForSeconds(SpawnCoolDown);
    }
    void Update()
    {
        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            int i=0,halfSpawnNum=SpawnNum/2;
            float dir = Random.Range(-180.0f, 180.0f), rot = 360.0f / halfSpawnNum;
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
                dir = Random.Range(-180.0f, 180.0f);
                moveVec.x = Mathf.Cos(dir);
                moveVec.y = Mathf.Sin(dir);
                StartCoroutine(SpawnExplosions(transform.position, moveVec, SpawnIts));
            }
        }
    }

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
