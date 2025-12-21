using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class JackMining : MonoBehaviour{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float moveSpeed = 5f;
    void Start(){
    }
    // Update is called once per frame
    void Update(){
        if (Keyboard.current[GlobalVariables.Instance.UpKey].isPressed){
            this.gameObject.transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        }
        if (Keyboard.current[GlobalVariables.Instance.DownKey].isPressed){
            this.gameObject.transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
        }
        if (Keyboard.current[GlobalVariables.Instance.MoveRightKey].isPressed){
            this.gameObject.transform.Translate(new Vector2(1,0) * moveSpeed * Time.deltaTime);
        }
        if (Keyboard.current[GlobalVariables.Instance.MoveLeftKey].isPressed){
            this.gameObject.transform.Translate(new Vector2(-1,0) * moveSpeed * Time.deltaTime);
        }
    }
}