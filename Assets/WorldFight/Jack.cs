using UnityEngine;
using UnityEngine.InputSystem;

public class Jack : MonoBehaviour{
    public Rigidbody2D rb;
    bool isGrounded = false;
    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Ground")){
            isGrounded = true;
        } 
    }
    void OnTriggerExit2D(Collider2D other){
        if (other.CompareTag("Ground")){
            isGrounded = false;
        }
    }

    void Update(){
        if (Keyboard.current[GlobalVariables.Instance.MoveLeftKey].isPressed){
            rb.linearVelocityX = -5f;
        }  
        else if (Keyboard.current[GlobalVariables.Instance.MoveRightKey].isPressed){
            rb.linearVelocityX = 5f; 
        }
        else{
            rb.linearVelocityX = 0f;
        }
        if(Keyboard.current[GlobalVariables.Instance.JumpKey].wasPressedThisFrame && isGrounded){
            rb.linearVelocityY = 5f;
        }
    }
}
