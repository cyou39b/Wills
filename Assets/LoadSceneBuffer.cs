using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneBuffer : MonoBehaviour{
    public string Next;
    void Start(){}
    void Update(){}
    void OnTriggerEnter2D(){
        EffectManager.Instance.TrueInstance.ClearAllEffect();
        LoadSceneManager.LoadBufferAndLoadScene(Next);
    }
}