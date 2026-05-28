using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManagerInMining : AbstractEffectManager{
    List<PossessionItems> nowEffect = new List<PossessionItems>();

    public override void Update(){
        List<PossessionItems> effectsNextFrame = new List<PossessionItems>();

        foreach(PossessionItems pItem in nowEffect){
            if(Time.time >= pItem.effect.endTime){
                ClearSPDUp(pItem);
                nowEffect.Remove(pItem);
            }
            else
            {
                effectsNextFrame.Add(pItem);
            }
        }

        nowEffect = effectsNextFrame;
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