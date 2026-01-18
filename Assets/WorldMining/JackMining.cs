using UnityEngine;
using UnityEngine.InputSystem;

public class JackMining : MonoBehaviour{
    public float moveSpeed = 5f;
    public GameObject Map;
    void Start(){}
    void Update(){
        if(!Map.activeSelf){
            if (Keyboard.current[GlobalVariables.Instance.UpKey].isPressed){
                gameObject.transform.Translate(Vector2.up * (moveSpeed * Time.deltaTime));
            }
            if (Keyboard.current[GlobalVariables.Instance.DownKey].isPressed){
                gameObject.transform.Translate(Vector2.down * (moveSpeed * Time.deltaTime));
            }
            if (Keyboard.current[GlobalVariables.Instance.MoveRightKey].isPressed){
                gameObject.transform.Translate(Vector2.right * (moveSpeed * Time.deltaTime));
            }
            if (Keyboard.current[GlobalVariables.Instance.MoveLeftKey].isPressed){
                gameObject.transform.Translate(Vector2.left * (moveSpeed * Time.deltaTime));
            }
        }
    }
}