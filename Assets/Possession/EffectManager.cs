using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//revive
//To instance AbstractEffectManager easily,AbstractEffectManager doesn't inherit MonoBehaviour
//use class "EffectManager" run start and update in unity
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
                case "WorldFight":
                    TrueInstance = new EffectManagerInFighting();
                    break;
                default:
                    break;
            }
            if(TrueInstance!=null){TrueInstance.Start();}
            SceneManager.activeSceneChanged += SceneChange;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    void Update(){
        if(TrueInstance!=null){TrueInstance.Update();}
    }

    void SceneChange(Scene prev, Scene curr)
    {
        switch(curr.name)
        {
            case "WorldMining":
                TrueInstance = new EffectManagerInMining();
                break;
            case "MainScene":
                TrueInstance = new EffectManagerInMainScene();
                break;
            case "WorldFight":
                TrueInstance = new EffectManagerInFighting();
                break;
            default:
                break;
        }
        if(TrueInstance!=null){TrueInstance.Start();}
    }
}