using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class JackMining : MonoBehaviour{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    public GameObject Map;

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

    void Start(){
        gameObject.transform.position = new Vector3(85.0f,25.0f,-1.0f);
        rb = GetComponent<Rigidbody2D>();
    }


    void Update(){
        if(!Map.activeSelf){
            int idx0, idx1;
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
        else{
            rb.linearVelocityX = 0f;
            rb.linearVelocityY = 0f;
            animator.speed = 0.0f;
        }
    }
}