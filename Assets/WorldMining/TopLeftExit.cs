using UnityEngine;
using UnityEngine.SceneManagement;

public class TopLeftExit : MonoBehaviour
{
    public GameObject Jack;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject != Jack){return;}

        Debug.Log("Jack Exit From Top Left, Loading MainScene...");
        SceneManager.LoadScene("MainScene");
    }
}
