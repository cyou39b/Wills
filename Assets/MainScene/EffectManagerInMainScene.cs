using UnityEngine;
using System.Collections.Generic;

public class EffectManagerInMainScene : AbstractEffectManager{
    List<PossessionItems> nowEffect = new List<PossessionItems>();
    void Start(){}
    void Update(){}
    public override void SPDUpFunc(PossessionItems pos){
        JackMainScene.moveSpeed *= pos.effect.EffectRate+1;
    }
    public override void ClearEffect(){
    }
}