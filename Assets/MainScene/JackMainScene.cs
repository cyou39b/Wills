using UnityEngine;
using UnityEngine.InputSystem;

public class JackMainScene : MonoBehaviour{
    public float moveSpeed = 3f;
    public Rigidbody2D rb;
    void Start(){}

    void Update(){
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
