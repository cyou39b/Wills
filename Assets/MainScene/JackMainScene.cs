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

    public new GameObject renderer;
    public Animator animator;
    private static readonly Quaternion facingLeftRotation = Quaternion.Euler(new Vector3(0.0f, -90.0f, 45.0f));
    private static readonly Quaternion facingRightRotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, -45.0f));
    private static readonly Quaternion facingDownRotation = Quaternion.Euler(new Vector3(45.0f, 180.0f, 0.0f));
    private static readonly Quaternion facingUpRotation = Quaternion.Euler(new Vector3(-45.0f, 0.0f, 0.0f));
    private Quaternion[][] rotations = new Quaternion[][]
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

    void Start(){}

    void Update(){
        if( !MenuManager.IsMenuOpen && 
            !DialogueManager.IsTalking && 
            !BackpackLogic.IsBackpackOpening &&
            !MenuInSceneLogic.IsSceneMenuOpen){
            int idx0,idx1;
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