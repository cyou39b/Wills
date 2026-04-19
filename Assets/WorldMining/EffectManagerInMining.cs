using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManagerInMining : AbstractEffectManager{
    List<PossessionItems> nowEffect = new List<PossessionItems>();
    public override void Update(){
        if(nowEffect != null){
            foreach(PossessionItems pos in nowEffect){
                if(Time.time >= pos.effect.endTime){
                    ClearSPDUp(pos);
                    nowEffect.Remove(pos);
                }
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
            items.effect.usedTimes--;
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