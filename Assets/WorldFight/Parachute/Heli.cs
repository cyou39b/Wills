using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Heli : MonoBehaviour
{
    public Vector3 Offset;
    private static Heli Instance = null;

    public Jack jack;
    public JackEntrance jackEntrance;

    private Rigidbody2D rb;
    public Vector2 Velocity;

    IEnumerator Start()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Velocity;
        jack.rb.bodyType = RigidbodyType2D.Static;
        // jack.transform.position = new Vector3(transform.position.x, transform.position.y, jack.transform.position.z) + Offset;
        jack.transform.localScale = new Vector3(0.01f, 0.01f);

        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

        jack.transform.parent = null;
        jack.gameObject.SetActive(true);
        jack.transform.position = new Vector3(transform.position.x, transform.position.y, jack.transform.position.z) - Offset;
        jack.rb.bodyType = RigidbodyType2D.Dynamic;
        jack.transform.localScale = new Vector3(2.2f, 2.2f);
        jackEntrance.called = false;
        jack.rb.linearVelocity = new Vector2(
            Random.Range(-jackEntrance.startVelocity.x, jackEntrance.startVelocity.x),
            Random.Range(float.Epsilon, jackEntrance.startVelocity.y)
        );

        yield return new WaitForSeconds(5.0f);

        transform.position = new Vector3(-9999, -9999, transform.position.z);
        enabled = false;
    }

    public static void StartWin()
    {
        Instance.StartWinAni();
    }
    void StartWinAni()
    {
        StartCoroutine(StartWinIt());
    }

    IEnumerator StartWinIt()
    {
        jack.EndPlayerAction();
        jack.enabled = false;
        jack.rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(1.5f);

        enabled = true;
        transform.position = new Vector3(jack.transform.position.x, 18.0f, transform.position.z) + Offset;
        rb.linearVelocity = new Vector2(0.0f, -2.0f);

        yield return new WaitUntil(() => jack.transform.position.y + Offset.y >= transform.position.y);
        
        jack.rb.bodyType = RigidbodyType2D.Kinematic;
        jack.transform.localScale = new Vector3(0.001f, 0.001f);
        jack.rb.linearVelocity = Vector2.zero;
        jack.transform.parent = transform;
        rb.linearVelocity = new Vector2(0.0f, 4.0f);
        RbCameraMovement.UseRB = false;

        yield return new WaitForSeconds(4.0f);

        GlobalVariables.Instance.mainScenePosition = new Vector3(-20.0f, 2.5f);
        LoadSceneManager.LoadBufferAndLoadScene("MainScene");
    }
}
