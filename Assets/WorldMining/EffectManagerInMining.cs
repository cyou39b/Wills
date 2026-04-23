using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManagerInMining : AbstractEffectManager{
    List<PossessionItems> nowEffect = new List<PossessionItems>();
    public override void Update(){
        for(int i = 0;i < nowEffect.Count;i++){
            if(Time.time >= nowEffect[i].effect.endTime){
                ClearSPDUp(nowEffect[i]);
                nowEffect.Remove(nowEffect[i]);
            }
        }
    }
    public override void SPDUpFunc(PossessionItems pos){
        JackMining.moveSpeed *= pos.effect.EffectRate+1;
        pos.effect.endTime = Time.time + pos.effect.duration;
        nowEffect.Add(pos);
    }
    public override void ClearAllEffect(){
        foreach(PossessionItems items in nowEffect){
            ClearSPDUp(items);
        }
        nowEffect.Clear();
    }
    public override void ClearSPDUp(PossessionItems pos){
        JackMining.moveSpeed /= pos.effect.EffectRate+1;
        pos.effect.usedTimes--;
    }
    public override Action GetPossessionEffectAction(PossessionItems pos){
        switch (pos.effect.effectType){
            case EffectType.SPDUp:
                return () => SPDUpFunc(pos);
            default:
                return () => {};
        }
    }
}