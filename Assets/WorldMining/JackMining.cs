using UnityEngine;
using UnityEngine.InputSystem;

public class JackMining : MonoBehaviour{
    public float moveSpeed = 5f;
    public GameObject Map;
    public Rigidbody2D rb;
    void Start(){}
    void Update(){
        if(!Map.activeSelf){
            if (Keyboard.current[GlobalVariables.Instance.UpKey].isPressed){
                rb.linearVelocityY = moveSpeed;
            }
            else if (Keyboard.current[GlobalVariables.Instance.DownKey].isPressed){
                rb.linearVelocityY = -moveSpeed;
            }
            else{
                rb.linearVelocityY = 0f;
            }
            if (Keyboard.current[GlobalVariables.Instance.MoveRightKey].isPressed){
                rb.linearVelocityX = moveSpeed;
            }
            else if (Keyboard.current[GlobalVariables.Instance.MoveLeftKey].isPressed){
                rb.linearVelocityX = -moveSpeed;
            }
            else{
                rb.linearVelocityX = 0f;
            }
        }
    }
}