using UnityEngine;
using System.Collections.Generic;
using System;

public class EffectManagerInMainScene : AbstractEffectManager{
    List<PossessionItems> nowEffect = new List<PossessionItems>();
    public override void Update(){
        List<PossessionItems> effectsNextFrame = new List<PossessionItems>();

        foreach(PossessionItems pItem in nowEffect){
            if(Time.time >= pItem.effect.endTime){
                ClearSPDUp(pItem);
            }
            else
            {
                effectsNextFrame.Add(pItem);
            }
        }

        nowEffect = effectsNextFrame;
    }

    public override void SPDUpFunc(PossessionItems pos){
        JackMainScene.moveSpeed *= pos.effect.EffectRate+1;
        pos.effect.endTime = Time.time + pos.effect.duration;
        Debug.Log($"{pos.effect.endTime}");
        nowEffect.Add(pos);
    }
    public override void ClearAllEffect(){
        foreach(PossessionItems items in nowEffect){
            ClearSPDUp(items);
        }
        nowEffect.Clear();
    }
    public override void ClearSPDUp(PossessionItems pos){
        JackMainScene.moveSpeed /= pos.effect.EffectRate+1;
        pos.effect.usedTimes--;
        Debug.Log("effect is cleared");
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