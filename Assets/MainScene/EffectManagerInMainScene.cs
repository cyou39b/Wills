using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class EffectManagerInMainScene : AbstractEffectManager{
    List<PossessionItems> nowEffect = new List<PossessionItems>();
    public override void Update(){
        if(nowEffect != null){
            foreach(PossessionItems items in nowEffect){
                if(Time.time >= items.effect.endTime){
                    ClearSPDUp(items);
                    nowEffect.Remove(items);
                }
            }
        }
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
            items.effect.usedTimes--;
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
                Debug.Log("return func");
                return () => SPDUpFunc(pos);
            default:
                return () => {};
        }
    }
}