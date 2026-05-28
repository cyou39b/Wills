using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class JackMainScene : MonoBehaviour{
    public static float moveSpeed = 5.0f;
    public Rigidbody2D rb;
    List<IInteract> inRange = new List<IInteract>();
    public Button Button1;
    public Button Button2;
    public Text Button1Text;
    public Text Button2Text;
    IInteract closerObj;
    IInteract closestObj;
    public static bool isInInteractRange1 = false;
    public static bool isInInteractRange2 = false;

    public new GameObject renderer;
    public Animator animator;

    public int idx0 = 1, idx1 = 0;
    public static Quaternion[][] rotations = new Quaternion[][]
    {
        new Quaternion[] // left
        {
            Quaternion.Euler(new Vector3(-32.973f, -57.444f, 40.446f)), // left up
            Quaternion.Euler(new Vector3(0.0f, -90.0f, 45.0f)), // left
            Quaternion.Euler(new Vector3(33.29f, -122.229f, 41.044f)), // left down
        },
        new Quaternion[] // not left or right
        {
            Quaternion.Euler(new Vector3(-45.0f, 0.0f, 0.0f)), // down
            Quaternion.identity,
            Quaternion.Euler(new Vector3(45.0f, 180.0f, 0.0f)), // up
        },
        new Quaternion[] // right
        {
            Quaternion.Euler(new Vector3(-32.464f, 56.937f, -39.508f)), // right up
            Quaternion.Euler(new Vector3(0.0f, 90.0f, -45.0f)), // right
            Quaternion.Euler(new Vector3(36.624f, 118.229f, -48.016f)), // right down
        },
    };

    void Start()
    {
        renderer.transform.rotation = rotations[idx0][idx1];
    }

    void Update(){
        if( !MenuManager.IsMenuOpen && 
            !DialogueManager.IsTalking && 
            !BackpackLogic.IsBackpackOpening &&
            !MenuInSceneLogic.IsSceneMenuOpen){
            if (Keyboard.current[GlobalVariables.Instance.UpKey].isPressed){
                rb.linearVelocityY = moveSpeed;
                idx1 = 0;
            }
            else if (Keyboard.current[GlobalVariables.Instance.DownKey].isPressed){
                rb.linearVelocityY = -moveSpeed;
                idx1 = 2;
            }
            else{
                rb.linearVelocityY = 0f;
                idx1 = 1;
            }
            if (Keyboard.current[GlobalVariables.Instance.MoveRightKey].isPressed){
                rb.linearVelocityX = moveSpeed;
                idx0 = 2;
            }
            else if (Keyboard.current[GlobalVariables.Instance.MoveLeftKey].isPressed){
                rb.linearVelocityX = -moveSpeed;
                idx0 = 0;
            }
            else{
                rb.linearVelocityX = 0f;
                idx0 = 1;
            }

            if(idx1 != 1 || idx0 != 1)
            {
                animator.speed = 1.2f;
                renderer.transform.rotation = rotations[idx0][idx1];
            }
            else
            {
                animator.speed = 0.0f;
            }
        }

        if (Keyboard.current[GlobalVariables.Instance.InteractKey].wasPressedThisFrame && 
            Button1.gameObject.activeSelf && 
            !MenuManager.IsMenuOpen && 
            !DialogueManager.IsTalking &&
            !BackpackLogic.IsBackpackOpening){
            DialogueManager.Instance.StartDialogue(closestObj);
        }
    }
        
    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Interact")){
            if(other.gameObject.TryGetComponent<IInteract>(out IInteract interact)){
                inRange.Add(interact);
            }
            Button1.gameObject.SetActive(true);
            isInInteractRange1 = true;
            if(inRange.Count > 1){
                Button2.gameObject.SetActive(true);
                isInInteractRange2 = true;
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
            if(Button1 == null && Button2 == null){return;}
            switch(inRange.Count){
                case 1 :
                    Button2.gameObject.SetActive(false);
                    isInInteractRange2 = false;
                    break;
                case 0 :
                    Button1.gameObject.SetActive(false);
                    Button2.gameObject.SetActive(false);
                    isInInteractRange1 = false;
                    isInInteractRange2 = false;
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