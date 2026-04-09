using System.Collections.Generic;
using UnityEngine;

public class EffectManagerInMining : AbstractEffectManager{
    List<PossessionItems> nowEffect = new List<PossessionItems>();
    void Start(){}
    void Update(){}
    public override void SPDUpFunc(PossessionItems pos){
        JackMining.moveSpeed *= pos.effect.EffectRate+1;
    }
    public override void ClearEffect(){
    }
}
