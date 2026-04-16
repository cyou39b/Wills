using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//revive
public class EffectManager : MonoBehaviour{
    public static EffectManager Instance{get;private set;} = null;
    public AbstractEffectManager TrueInstance = null;
    void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        else{
            Instance = this;
            switch (SceneManager.GetActiveScene().name){
                case "WorldMining":
                    TrueInstance = new EffectManagerInMining();
                    break;
                case "MainScene":
                    TrueInstance = new EffectManagerInMainScene();
                    break;
                case "WorldFighting":
                    TrueInstance = new EffectManagerInFighting();
                    break;
                default:
                    break;
            }
            DontDestroyOnLoad(this.gameObject);
        }
    }
    void Start(){
        TrueInstance.Start();
    }
    void Update(){
        TrueInstance.Update();
    }
}