using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class JackMainScene : MonoBehaviour{
    public float moveSpeed = 5.0f;
    public Rigidbody2D rb;
    List<IInteract> inRange = new List<IInteract>();
    public Button Button1;
    public Button Button2;
    public Text Button1Text;
    public Text Button2Text;
    IInteract closerObj;
    IInteract closestObj;
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
        if (Keyboard.current[GlobalVariables.Instance.InteractKey].wasPressedThisFrame && Button1.gameObject.activeSelf){
            DialogueManager.Instance.StartDialogue(closestObj);
        }
    }
        
    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Interact")){
            if(other.gameObject.TryGetComponent<IInteract>(out IInteract interact)){
                inRange.Add(interact);
            }
            Button1.gameObject.SetActive(true);
            if(inRange.Count > 1){
                Button2.gameObject.SetActive(true);
            }
            FindClosestAndCloser();
        }
    }
    void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag("Interact")){
            if(other.gameObject.TryGetComponent<IInteract>(out IInteract interact)){
                inRange.Remove(interact);
            }
            if(inRange.Count >=1){
                FindClosestAndCloser();
            }
            switch(inRange.Count){
                case 1 :
                    Button2.gameObject.SetActive(false);
                    break;
                case 0 :
                    Button1.gameObject.SetActive(false); // (sometimes)MissingReference will be thrown but don't affect during game
                    Button2.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }
    void FindClosestAndCloser(){
        Button1.onClick.RemoveAllListeners();
        Button2.onClick.RemoveAllListeners();

        closestObj = null;
        closerObj = null;

        List<IInteract> ordered = inRange.OrderBy(i => Vector2.Distance(gameObject.transform.position,i.GetPosition())).ToList();

        closestObj = ordered[0];
        Button1Text.text = closestObj.WriteInteractText();
        Button1.onClick.AddListener(closestObj.OnButtonClick);

        if (inRange.Count > 1){
            closerObj = ordered[1];
            Button2Text.text = closerObj.WriteInteractText();
            Button2.onClick.AddListener(closerObj.OnButtonClick);
        }
    }
}