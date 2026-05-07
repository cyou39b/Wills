using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    private static int _uid = 0;
    public static int EnemyUid
    {
        get
        {
            return _uid++;
        }
    }

    public Rect spawnRange;
    public List<WeightedEnemyPrefab> enemyPrefabs = new List<WeightedEnemyPrefab>();
    public static string Log = "";
    public static int spawnCnt = 1;

    public static HashSet<AbstractEnemy> AllEnemys = new HashSet<AbstractEnemy>();

    void Start()
    {
        for(int i=0;i<spawnCnt;i++)
        {
            SpawnOneEnemy();
        }
    }

    void SpawnOneEnemy()
    {
        float v = Random.Range(0.0f, 1.0f);
        int i;
        for(i=0;;i++)
        {
            v -= enemyPrefabs[i].weight;
            if(v <= 0.0f) {break;}
        }

        Vector3 pos = new Vector3(Random.Range(spawnRange.xMin, spawnRange.xMax), Random.Range(spawnRange.yMin, spawnRange.yMax), 0.0f);
        Instantiate(enemyPrefabs[i].prefab, pos, Quaternion.identity);
    }

    [ContextMenu("Log Enemys")]
    void r()
    {
        // StringBuilder sb = new StringBuilder(Log);
        // foreach(AbstractEnemy enemy in AllEnemys)
        // {
        //     sb.Append(enemy.name);
        //     sb.Append(": {");
        //     foreach(AbstractEnemy other in enemy.slowedEnemy)
        //     {
        //         sb.Append(other.name);
        //         sb.Append(", ");
        //     }
        //     sb.Append("}\n");
        // }
        // Debug.Log(sb.ToString());
    }

    void OnDrawGizmosSelected()
    {
        Vector3 topLeft = transform.TransformPoint(new Vector3(spawnRange.xMin, spawnRange.yMax, 0));
        Vector3 topRight = transform.TransformPoint(new Vector3(spawnRange.xMax, spawnRange.yMax, 0));
        Vector3 bottomLeft = transform.TransformPoint(new Vector3(spawnRange.xMin, spawnRange.yMin, 0));
        Vector3 bottomRight = transform.TransformPoint(new Vector3(spawnRange.xMax, spawnRange.yMin, 0));

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    public static void CheckIfAllEnemyDied()
    {
        if(AllEnemys.Count == 0)
        {
            Heli.StartWin();
        }
    }
}

[Serializable]
public class WeightedEnemyPrefab
{
    public GameObject prefab;
    public float weight;
}