using UnityEngine;
using UnityEngine.InputSystem;

public class Jack : MonoBehaviour{
    public Rigidbody2D rb;
    bool isGrounded = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
    }
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
    // Update is called once per frame
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
        if(Keyboard.current[GlobalVariables.Instance.JumpKey].wasPressedThisFrame && isGrounded == true){
            rb.linearVelocityY = 5f;
        }
    }
}
