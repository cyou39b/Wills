using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class MainCam : MonoBehaviour
{
    private Camera cam;
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if(Keyboard.current.numpadPlusKey.wasPressedThisFrame)
        {
            cam.orthographicSize += 0.1f;
        }
        if(Keyboard.current.numpadMinusKey.wasPressedThisFrame)
        {
            cam.orthographicSize -= 0.1f;
        }
    }
}
