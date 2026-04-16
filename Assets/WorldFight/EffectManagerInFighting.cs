using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class EffectManagerInFighting : AbstractEffectManager{
    List<PossessionItems> nowEffect = new List<PossessionItems>();
    public GameObject Player;
    Jack script;
    public AbstractEnemy enemyScript;
    public override void Start(){
        if(Player.TryGetComponent<Jack>(out Jack jack)){
            script = jack;
        }
    }
    public override void Update(){
        foreach(PossessionItems pos in nowEffect){
            if(Time.time >= pos.effect.endTime){
                if(pos.effect.effectType == EffectType.ATKBoost){
                    ClearSPDUp(pos);
                    nowEffect.Remove(pos);
                }
                else if(pos.effect.effectType == EffectType.ATKBoost){
                    ClearATKBoost(pos);
                    nowEffect.Remove(pos);
                }
                else if(pos.effect.effectType == EffectType.BulletKnockbackForceUp){
                    ClearBulletKnockbackForceUpFunc(pos);
                    nowEffect.Remove(pos);
                }
            }
        }
    }

    public override void SPDUpFunc(PossessionItems pos){
        Jack.MoveSpeed *= pos.effect.EffectRate+1;
        pos.effect.endTime = Time.time + pos.effect.duration;
        nowEffect.Add(pos);
    }
    public override void ATKBoostFunc(PossessionItems pos){
        Bullet.Damage *= pos.effect.EffectRate+1;
        pos.effect.endTime = Time.time + pos.effect.duration;
        nowEffect.Add(pos);
    }
    public override void PlayerHPBoostFunc(PossessionItems pos){
        script.HpBar.MaxHP *= pos.effect.EffectRate+1;
        pos.effect.endTime = float.PositiveInfinity;
        nowEffect.Add(pos);
    }
    public override void EnemyHPUPFunc(PossessionItems pos){
        enemyScript.HpBar.MaxHP *= pos.effect.EffectRate+1;
        pos.effect.endTime = float.PositiveInfinity;
        nowEffect.Add(pos);
    }
    public override void BulletKnockbackForceUpFunc(PossessionItems pos){
        Bullet.Power *= pos.effect.EffectRate+1;
        pos.effect.endTime = Time.time + pos.effect.duration;
        nowEffect.Add(pos);
    }
    public override void ClearAllEffect(){
        foreach(PossessionItems item in nowEffect){
            switch (item.effect.effectType){
                case EffectType.ATKBoost:
                    ClearATKBoost(item);
                    break;
                case EffectType.PlayerHPBoost:
                    ClearPlayerHPBoost(item);
                    break;
                case EffectType.SPDUp:
                    ClearSPDUp(item);
                    break;
                case EffectType.EnemyHPUP:
                    ClearEnemyHPUP(item);
                    break;
                case EffectType.BulletKnockbackForceUp:
                    ClearBulletKnockbackForceUpFunc(item);
                    break;
                default:
                    break;
            }
        }
        nowEffect.Clear();
    }
    public override void ClearSPDUp(PossessionItems pos){
        Jack.MoveSpeed /= pos.effect.EffectRate+1;
    }
    public override void ClearPlayerHPBoost(PossessionItems pos){
        script.HpBar.MaxHP /= pos.effect.EffectRate+1;
    }
    public override void ClearATKBoost(PossessionItems pos){
        Bullet.Damage /= pos.effect.EffectRate+1;
    }
    public override void ClearEnemyHPUP(PossessionItems pos){
        enemyScript.HpBar.MaxHP /= pos.effect.EffectRate+1;
    }
    public override void ClearBulletKnockbackForceUpFunc(PossessionItems pos){
        Bullet.Power /= pos.effect.EffectRate+1;
    }
    public override Action GetPossessionEffectAction(PossessionItems pos){
        switch (pos.effect.effectType){
            case EffectType.ATKBoost:
                return () => ATKBoostFunc(pos);
            case EffectType.PlayerHPBoost:
                return () => PlayerHPBoostFunc(pos);
            case EffectType.SPDUp:
                return () => SPDUpFunc(pos);
            case EffectType.EnemyHPUP:
                return () => EnemyHPUPFunc(pos);
            case EffectType.BulletKnockbackForceUp:
                return () => BulletKnockbackForceUpFunc(pos);
            default:
                return () => {};
        }
    }
}