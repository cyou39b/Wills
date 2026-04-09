using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Dead
public class EffectManager : MonoBehaviour{
    HPBar JackHPBar;
    public static EffectManager Instance{get;private set;} = null;
    //public Dictionary<EffectType,Action> EffectWay = new Dictionary<EffectType, Action>();
    public List<PossessionItems> nowEffect = new List<PossessionItems>(); // Before scene change,the List will be cleared
    void Awake(){
        if(Instance == null || Instance == this){
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
            return;
        }
    }
    void Start(){
        GameObject Player = GameObject.FindWithTag("Player");
    }
    void Update(){}
    public void ATKBoostFunc(PossessionItems pos){
        pos.effect.usedTimes++;
        Bullet.Damage *= (pos.effect.EffectRate+1);
    }
    public void SPDUpFunc(PossessionItems pos){
        pos.effect.usedTimes++;
        string NowScene = SceneManager.GetActiveScene().name;
        switch (NowScene){
            case "WorldFight":
                break;
            case "WorldMining":
                JackMining.moveSpeed*= (pos.effect.EffectRate+1);
                break;
            case "MainScene":
                JackMainScene.moveSpeed *= (pos.effect.EffectRate+1);
                break;
            default:
                Debug.LogError("I don't know why you can use it......");
                break;
        }
    }
    public void HPBoostFunc(PossessionItems pos){
        pos.effect.usedTimes++;
    }
    //it will be called when the effect(s) end(s) or before the scene change
    public void ResetItem(PossessionItems pos){
        string NowScene = SceneManager.GetActiveScene().name;
        if (pos.effect.stackable){
            pos.effect.usedTimes = 0;
        }
        else{
            pos.effect.usedTimes--;
        }
        switch (pos.effect.effectType){
            case EffectType.SPDUp:
                switch (NowScene){
                    case "WorldFight":
                        break;
                    case "MainScene":
                        JackMainScene.moveSpeed = 5.0f;
                        break;
                    case "WorldMining":
                        JackMining.moveSpeed = 5.0f;
                        break;
                    default:
                        Debug.LogError("The function is called in wrong scene.");
                        break;
                }
                break;
            case EffectType.HPBoost:
                
                break;
            case EffectType.ATKBoost:
                Bullet.Damage = 5.0f;
                break;
            default:
                break;
        }
        nowEffect.Remove(pos);
    }
}