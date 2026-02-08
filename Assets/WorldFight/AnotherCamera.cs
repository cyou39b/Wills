using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AnotherCamera : MonoBehaviour
{
    public GameObject Followee;
    public Vector3 Diff;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void FixedUpdate()
    {
        if(Followee != null)
        {
            cam.enabled = true;
            transform.position = Followee.transform.position + Diff;
        }
        else
        {
            cam.enabled = false;
        }
    }
}
