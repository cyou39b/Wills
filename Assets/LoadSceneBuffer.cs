using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneBuffer : MonoBehaviour{
    void Start(){}
    void Update(){}
    void OnTriggerEnter2D(){
        LoadSceneManager.NextScene = gameObject.tag;
        SceneManager.LoadScene("LoadSceneBuffer");
    }
}