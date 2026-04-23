using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class EffectManagerInMainScene : AbstractEffectManager{
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
                Debug.Log("return func");
                return () => SPDUpFunc(pos);
            default:
                return () => {};
        }
    }
}