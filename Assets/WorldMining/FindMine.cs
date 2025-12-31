using UnityEngine;
using UnityEngine.InputSystem;

public class FindMine : MonoBehaviour{
    public SpriteRenderer sr;
    public GameObject player;
    public static bool IsDetect{get;private set;}
    public float Dis = 0;
    //public Generate DisList;
    void Start(){
        player = GameObject.FindWithTag("Player");
        sr.enabled = false;
        IsDetect = false ;
    }
    // Update is called once per frame
    void Update(){
        CalDis();
        if (Dis < 5f){
            IsDetect = true;
        }
        if (Keyboard.current[GlobalVariables.Instance.FindMine].wasPressedThisFrame && Dis < 2f){
            sr.enabled = true;
        }
        if (Dis < 1f && Keyboard.current[GlobalVariables.Instance.PickUpMine].wasPressedThisFrame && sr.enabled == true){
            Generate.Score1 +=1;
            Destroy(gameObject);
        }
    }
    void CalDis(){
        float xDis = (gameObject.transform.position.x - player.transform.position.x) * (gameObject.transform.position.x - player.transform.position.x);
        float yDis = (gameObject.transform.position.y - player.transform.position.y) * (gameObject.transform.position.y - player.transform.position.y);
        Dis = xDis + yDis;
    }
}