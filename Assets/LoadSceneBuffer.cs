using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneBuffer : MonoBehaviour{
    void Start(){}
    void Update(){}
    void OnTriggerEnter2D(){
        // Tag is not a random string field, I don't care you're lasy or waht, just don't use it this way!!
        LoadSceneManager.NextScene = gameObject.tag;
        SceneManager.LoadScene("LoadSceneBuffer");
    }
}