using UnityEngine;
using UnityEngine.InputSystem;

public class ExplodeTrigger : MonoBehaviour
{
    void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Explode.ExplodePosition = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
}
