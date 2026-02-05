using UnityEngine;
using UnityEngine.SceneManagement;

//Deprecated
public class BotRightExit : MonoBehaviour
{
    public GameObject Jack;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject != Jack){return;}

        Debug.Log("Jack Exit From Bot Right, Loading MainScene...");
        SceneManager.LoadScene("MainScene");
    }
}
