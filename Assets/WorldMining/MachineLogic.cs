using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MachineLogic : MonoBehaviour{
    public GameObject MinePrefab;
    public Sprite[] MineSprites;
    public float MineMinDistance, MineMaxDistance;
    public int TotalMinePerSpawn;
    public List<Vector2> UndetectedMines = new List<Vector2>();
    public List<GameObject> DetectedMines = new List<GameObject>();
    public Text NumMineText;

    public AudioSource MineNearbySFX;
    public float PlayMineNearbySFXRangeRadius;
    private float mineSFXCoolDown = 0.0f;
    
    public float DetectRangeRadius;
    public float MinableRangeRadius;

    public float NEAREST = 1000000;

    void Start()
    {
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

        if (Keyboard.current[GlobalVariables.Instance.FindMine].wasPressedThisFrame)
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

        if (Keyboard.current[GlobalVariables.Instance.PickUpMine].wasPressedThisFrame)
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
        UpdateNumMinesText();
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
        for(int i=0;i<10;i++){
            float rot = Random.Range(-Mathf.PI,Mathf.PI);
            float distance = Random.Range(5f,20f);
            float offsetx = Mathf.Cos(rot) * distance;
            float offsety = Mathf.Sin(rot) * distance;
            Vector2 newPos = new Vector2(
                gameObject.transform.position.x + offsetx,
                gameObject.transform.position.y + offsety
            );

            UndetectedMines.Add(newPos);
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
        NumMineText.text = $"Mine: {GlobalVariables.Instance.NumMines}";
    }
}
