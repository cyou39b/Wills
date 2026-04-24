using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class EffectManagerInFighting : AbstractEffectManager{
    List<PossessionItems> nowEffect = new List<PossessionItems>();
    private GameObject Player;
    Jack script;
    //public AbstractEnemy enemyScript;
    public override void Start(){
        Player = GameObject.FindWithTag("Player");
        if(Player == null)
        {
            Debug.LogError("Can't find player (gameObject named \"Jack\") in scene.");
        }

        if(Player.TryGetComponent<Jack>(out Jack jack)){
            script = jack;
        }
    }
    //that's weird,though
    public override IEnumerator LateStart(){
        yield return null;
    }
    public override void Update(){
        for(int i = 0;i < nowEffect.Count;i++){
            if(Time.time >= nowEffect[i].effect.endTime){
                if(nowEffect[i].effect.effectType == EffectType.SPDUp){
                    ClearSPDUp(nowEffect[i]);
                    nowEffect.Remove(nowEffect[i]);
                }
                else if(nowEffect[i].effect.effectType == EffectType.ATKBoost){
                    ClearATKBoost(nowEffect[i]);
                    nowEffect.Remove(nowEffect[i]);
                }
                else if(nowEffect[i].effect.effectType == EffectType.BulletKnockbackForceUp){
                    ClearBulletKnockbackForceUpFunc(nowEffect[i]);
                    nowEffect.Remove(nowEffect[i]);
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
        //enemyScript.HpBar.MaxHP *= pos.effect.EffectRate+1;
        foreach(AbstractEnemy enemy in EnemySpawner.AllEnemys){
            enemy.HpBar.MaxHP *= pos.effect.EffectRate+1;
            enemy.HpBar.HP = enemy.HpBar.MaxHP;
        }
        pos.effect.endTime = float.PositiveInfinity;
        nowEffect.Add(pos);
    }
    public override void BulletKnockbackForceUpFunc(PossessionItems pos){
        Bullet.Power *= pos.effect.EffectRate+1;
        pos.effect.endTime = Time.time + pos.effect.duration;
        nowEffect.Add(pos);
    }
    public override void ClearAllEffect(){
        if(nowEffect != null){
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
    }
    public override void ClearSPDUp(PossessionItems pos){
        Jack.MoveSpeed /= pos.effect.EffectRate+1;
        pos.effect.usedTimes--;
    }
    public override void ClearPlayerHPBoost(PossessionItems pos){
        script.HpBar.MaxHP /= pos.effect.EffectRate+1;
        pos.effect.usedTimes--;
    }
    public override void ClearATKBoost(PossessionItems pos){
        Bullet.Damage /= pos.effect.EffectRate+1;
        pos.effect.usedTimes--;

    }
    public override void ClearEnemyHPUP(PossessionItems pos){
        //enemyScript.HpBar.MaxHP /= pos.effect.EffectRate+1;
        foreach(AbstractEnemy enemy in EnemySpawner.AllEnemys){
            enemy.HpBar.MaxHP /= pos.effect.EffectRate+1;
        }
        pos.effect.usedTimes--;
    }
    public override void ClearBulletKnockbackForceUpFunc(PossessionItems pos){
        Bullet.Power /= pos.effect.EffectRate+1;
        pos.effect.usedTimes--;
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