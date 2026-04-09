using UnityEngine;
using System.Collections.Generic;

public class EffectManagerInFighting : AbstractEffectManager{
    List<PossessionItems> nowEffect = new List<PossessionItems>();
    public GameObject Player;
    
    void Start(){}

    void Update(){}
    public override void SPDUpFunc(PossessionItems pos){
        Jack.MoveSpeed *= pos.effect.EffectRate+1;
    }
    public override void ATKBoostFunc(PossessionItems pos){
        Bullet.Damage *= pos.effect.EffectRate+1;
    }
    public override void HPBoostFunc(PossessionItems pos){
    }
    public override void HPUPFunc(PossessionItems pos){
        
    }
    
    public override void ClearEffect(){
        foreach(PossessionItems item in nowEffect){
            switch (item.effect.effectType){
                case EffectType.none:
                case EffectType.ATKBoost:

                default:
                    break;
            }
        }
    }
}