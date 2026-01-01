using UnityEngine;
using UnityEngine.InputSystem;

public class JackMining : MonoBehaviour{
    public float moveSpeed = 5f;
    void Start(){}

    void Update(){
        if (Keyboard.current[GlobalVariables.Instance.UpKey].isPressed){
            this.gameObject.transform.Translate(Vector2.up * (moveSpeed * Time.deltaTime));
        }
        if (Keyboard.current[GlobalVariables.Instance.DownKey].isPressed){
            this.gameObject.transform.Translate(Vector2.down * (moveSpeed * Time.deltaTime));
        }
        if (Keyboard.current[GlobalVariables.Instance.MoveRightKey].isPressed){
            this.gameObject.transform.Translate(Vector2.right * (moveSpeed * Time.deltaTime));
        }
        if (Keyboard.current[GlobalVariables.Instance.MoveLeftKey].isPressed){
            this.gameObject.transform.Translate(Vector2.left * (moveSpeed * Time.deltaTime));
        }
    }
}