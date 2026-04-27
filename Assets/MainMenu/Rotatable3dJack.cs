using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class Rotatable3dJack : MonoBehaviour
{

    // if you're wondering what's with this 'new' keyword
    // basically unity used to have collider under every componet in old versions for some reason
    // they got obsolete in later versions but the field is still there, it just became a pure throw function
    // if u don't add this 'new' keyword the LSP will be mad
    private new Collider collider;

    private Camera cam;

    private bool isHovered;
    private RaycastHit rayCastInfo;
    private bool getGrabbed = false;
    private Vector3 prevFrameMousePosition;

    private MenuSoundEffect soundEffect;
    private Vector3 angularVelocity;

    public GameObject OptionMenu;

    void Start()
    {
        collider = GetComponent<Collider>();
        cam = Camera.main;

        angularVelocity = new Vector3(
            Random.Range(-1.0f, 1.0f),
            Random.Range(-1.0f, 1.0f),
            Random.Range(-1.0f, 1.0f)
        );
        prevFrameMousePosition = Mouse.current.position.ReadValue();

        GameObject soundEffectGameObject = GameObject.Find("Audio Source");
        if(!soundEffectGameObject.TryGetComponent<MenuSoundEffect>(out soundEffect))
        {
            Debug.LogError("missing component!!");
        }
    }

    void Update()
    {
        Mouse currentMouse = Mouse.current;
        Vector3 mouseScreenPos = currentMouse.position.ReadValue();

        Ray mouseRay = cam.ScreenPointToRay(mouseScreenPos);
        if(!OptionMenu.activeInHierarchy && Physics.Raycast(mouseRay, out rayCastInfo)){isHovered = rayCastInfo.collider == this.collider;}
        else{isHovered = false;}

        if(currentMouse.leftButton.wasReleasedThisFrame && getGrabbed) {
            getGrabbed = false;
            Vector3 diff = mouseScreenPos - prevFrameMousePosition;
            angularVelocity = new Vector3(
                diff.y,-diff.x,0.0f
            ) * (0.02f / Time.deltaTime);
        }
        else if(Mouse.current.leftButton.wasPressedThisFrame && isHovered)
        {
            getGrabbed = true;
            angularVelocity = Vector3.zero;
        }
        else if(getGrabbed)
        {
            Vector3 diff = mouseScreenPos - prevFrameMousePosition;
            transform.Rotate(
                new Vector3(
                    diff.y,-diff.x,0.0f
                ),
                Space.World
            );
        }

        prevFrameMousePosition = mouseScreenPos;

        if(angularVelocity.magnitude >= 20.0f)
        {
            soundEffect.PlaySoundEffectIfPossible();           
        }
    }

    void FixedUpdate()
    {
        if(!getGrabbed)
        {
            transform.Rotate(angularVelocity, Space.World);
        }
    }
}
