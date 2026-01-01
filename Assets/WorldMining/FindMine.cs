using UnityEngine;
using UnityEngine.InputSystem;

//! WARN: Deprecated file, moved to Machine Logic
public class FindMine : MonoBehaviour{
    public SpriteRenderer sr;
    public GameObject player;
    public static bool IsDetect{get;private set;}
    public float Dis = 0;
    public float DetectRange = 5.0f;
    public float MinableRange = 1.0f;
    void Start(){
        player = GameObject.FindWithTag("Player");
        sr.enabled = false;
        IsDetect = false ;
    }
    void Update(){
        CalDis();
        if (Dis < DetectRange){
            IsDetect = true;
        }
        else
        {
            IsDetect = false;
        }
        if (Keyboard.current[GlobalVariables.Instance.FindMine].wasPressedThisFrame && Dis < 2f){
            sr.enabled = true;
        }
        if (Dis < MinableRange && Keyboard.current[GlobalVariables.Instance.PickUpMine].wasPressedThisFrame && sr.enabled == true){
            GlobalVariables.Instance.NumMines++;
            Destroy(gameObject);
        }
    }
    void CalDis(){
        float xDis = (gameObject.transform.position.x - player.transform.position.x) * (gameObject.transform.position.x - player.transform.position.x);
        float yDis = (gameObject.transform.position.y - player.transform.position.y) * (gameObject.transform.position.y - player.transform.position.y);
        Dis = xDis + yDis;
    }
}