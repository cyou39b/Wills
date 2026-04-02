using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneBuffer : MonoBehaviour{
    public string Next;
    void Start(){}
    void Update(){}
    void OnTriggerEnter2D(){
        // Tag is not a random string field, I don't care you're lasy or waht, just don't use it this way!!
        // and you had wrong spelling
        LoadSceneManager.NextScene = Next;
        foreach(PossessionItems possession in EffectManager.Instance.nowEffect){
            EffectManager.Instance.ResetItem(possession);
        }
        SceneManager.LoadScene("LoadSceneBuffer");
    }
}