using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneBuffer : MonoBehaviour{
    public string Next;
    void Start(){}
    void Update(){}
    void OnTriggerEnter2D(){
        EffectManager.Instance.TrueInstance.ClearAllEffect();
        GlobalVariables.Instance.mainScenePosition = new Vector3(7.0f, 20.0f);
        LoadSceneManager.LoadBufferAndLoadScene(Next);
    }
}