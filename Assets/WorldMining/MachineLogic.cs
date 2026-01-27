using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// 

public class MachineLogic : MonoBehaviour{
    public GameObject MinePrefab;
    public Sprite[] MineSprites;
    public int TotalMinePerSpawn;
    public List<Vector2> UndetectedMines = new List<Vector2>();
    // 儲存還沒被detect的ore
    public List<GameObject> DetectedMines = new List<GameObject>();
    // 儲存已經被detect的ore
    public Text NumMineText;

    // ore可以生成的長方形範圍，用兩個Vector2去標記(不知道為啥是左上右下而不是左下右上)
    public Vector2 ButtomRight;
    public Vector2 TopLeft;
    public Rect[] Rects;
    public RectRandom RectRandomMachine;

    public AudioSource MineNearbySFX;
    public float PlayMineNearbySFXRangeRadius;
    private float mineSFXCoolDown = 0.0f;
    
    public float DetectRangeRadius;
    public float MinableRangeRadius;

    public float NEAREST = float.PositiveInfinity;

    void Start()
    {
        RectRandomMachine = new RectRandom(Rects);
        SpawnMines();
        UpdateNumMinesText();
    }

    void Update()
    {
        if (Keyboard.current.shiftKey.wasPressedThisFrame){
            RemoveAllMines();
            SpawnMines();
            return;
        }


        float minDistanceToAllMines = float.PositiveInfinity;

        if (Keyboard.current[GlobalVariables.Instance.FindMineKey].wasPressedThisFrame)
        {
            List<Vector2> undetectedMinesAfterDetection = new List<Vector2>();
            foreach (Vector2 minePos in UndetectedMines)
            {
                float distance = 
                    (minePos.x - transform.position.x) * (minePos.x - transform.position.x) +
                    (minePos.y - transform.position.y) * (minePos.y - transform.position.y);
                minDistanceToAllMines = Mathf.Min(distance, minDistanceToAllMines);
                if(distance <= DetectRangeRadius)
                {

                    DetectedMines.Add(InstantiateMine(minePos));
                }
                else
                {
                    undetectedMinesAfterDetection.Add(minePos);
                }
            }
            UndetectedMines = undetectedMinesAfterDetection;
        }
        else
        {
            foreach (Vector2 minePos in UndetectedMines)
            {
                float distance = 
                    (minePos.x - transform.position.x) * (minePos.x - transform.position.x) +
                    (minePos.y - transform.position.y) * (minePos.y - transform.position.y);
                minDistanceToAllMines = Mathf.Min(distance, minDistanceToAllMines);
            }

        }

        if (Keyboard.current[GlobalVariables.Instance.PickUpMineKey].wasPressedThisFrame)
        {
            List<GameObject> detectedMinesAfterPickup = new List<GameObject>();
            foreach (GameObject mineObj in DetectedMines)
            {
                float distance = 
                    (mineObj.transform.position.x - transform.position.x) * (mineObj.transform.position.x - transform.position.x) + 
                    (mineObj.transform.position.y - transform.position.y) * (mineObj.transform.position.y - transform.position.y);
                minDistanceToAllMines = Mathf.Min(distance, minDistanceToAllMines);
                if(distance <= MinableRangeRadius)
                {
                    Destroy(mineObj);
                    GlobalVariables.Instance.NumMines++;
                    UpdateNumMinesText();
                }
                else
                {
                    detectedMinesAfterPickup.Add(mineObj);
                }
            }
            DetectedMines = detectedMinesAfterPickup;
        }
        else
        {
            foreach (GameObject mineObj in DetectedMines)
            {
                float distance = 
                    (mineObj.transform.position.x - transform.position.x) * (mineObj.transform.position.x - transform.position.x) + 
                    (mineObj.transform.position.y - transform.position.y) * (mineObj.transform.position.y - transform.position.y);
                minDistanceToAllMines = Mathf.Min(distance, minDistanceToAllMines);
            }
        }

        mineSFXCoolDown -= Time.deltaTime;
        if(minDistanceToAllMines <= PlayMineNearbySFXRangeRadius && mineSFXCoolDown <= 0.0f)
        {
            mineSFXCoolDown = Mathf.Max(minDistanceToAllMines * 0.3f, 0.5f);
            MineNearbySFX.Play();           
        }
        NEAREST = minDistanceToAllMines;
    }

    public GameObject InstantiateMine(Vector2 minePos)
    {
        GameObject newObj = Instantiate(
            MinePrefab,
            minePos,
            Quaternion.identity // identity is just (0f, 0f, 0f, 0f)
        );
        
        SpriteRenderer newObjSprerr;
        if(newObj.TryGetComponent<SpriteRenderer>(out newObjSprerr))
        {
            newObjSprerr.sprite = MineSprites[Random.Range(0, MineSprites.Length)];
        }
        else
        {
            Debug.LogWarning("Mine game object created doesn't have Sprite Renderer");
        }
        return newObj;
    }

    [ContextMenu("Spawn Mines")]
    void SpawnMines()
    {
        for(int i = 0; i < TotalMinePerSpawn; i++)
        {
            UndetectedMines.Add(RectRandomMachine.GetPoint());
        }
    }

    [ContextMenu("Remove All Mines")]
    void RemoveAllMines()
    {
        UndetectedMines.Clear();

        foreach(GameObject obj in DetectedMines)
        {
            Destroy(obj);
        }
        DetectedMines.Clear();
    }

    void UpdateNumMinesText()
    {
        if (MenuManager.IsMenuOpen || MapScenesSwicher.isMapOpening){
            NumMineText.enabled = false;
        }
        else{
            NumMineText.enabled = true;
        }
        NumMineText.text = $"Mine: {GlobalVariables.Instance.NumMines}";
    }
}

public class RectRandom
{
    private Rect[] rects;
    private float[] areas;
    private float totalArea;

    public RectRandom(Rect[] _rects) 
    {
        if(_rects.Length == 0)
        {
            throw new ArgumentException("argument shouldn't be empty", "_rects");
        }

        totalArea = 0.0f;
        rects = _rects;
        
        areas = new float[_rects.Length];
        for(int i=0;i<_rects.Length;i++)
        {
            float area = Mathf.Abs(_rects[i].size.x * _rects[i].size.y);
            totalArea += area;
            areas[i] = area;
        }
    }

    public Vector2 GetPoint()
    {
        float w = Random.Range(0.0f, totalArea);
        for(int i=0;i<areas.Length;i++)
        {
            if(w<=areas[i])
            {
                return new Vector2(
                    Random.Range(rects[i].xMin, rects[i].xMax),
                    Random.Range(rects[i].yMin, rects[i].yMax)
                );
            }
            w -= areas[i];
        }

        Debug.Log("Something went wrong with floating point arithmetic but this should be fine...?");
        return new Vector2(
            Random.Range(rects[0].xMin, rects[0].xMax),
            Random.Range(rects[0].yMin, rects[0].yMax)
        );
    }
}