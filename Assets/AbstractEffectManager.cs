using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractEffectManager{
    public virtual void Start(){}
    public abstract void Update();

    public virtual void ATKBoostFunc(PossessionItems pos){}
    public abstract void SPDUpFunc(PossessionItems pos);
    public virtual void PlayerHPBoostFunc(PossessionItems pos){}
    public virtual void EnemyHPUPFunc(PossessionItems pos){}
    public virtual void BulletKnockbackForceUpFunc(PossessionItems pos){}
    public virtual void HealPlayerFunc(PossessionItems pos){}
    public virtual void HealEnemyFunc(PossessionItems pos){}

    public abstract void ClearAllEffect();
    public abstract void ClearSPDUp(PossessionItems pos);
    public virtual void ClearATKBoost(PossessionItems pos){}
    public virtual void ClearPlayerHPBoost(PossessionItems pos){}
    public virtual void ClearEnemyHPUP(PossessionItems pos){}
    public virtual void ClearBulletKnockbackForceUpFunc(PossessionItems pos){}
    public virtual void ClearHealPayer(PossessionItems pos){}
    public virtual void ClearHealEnemy(PossessionItems pos){}
    public abstract Action GetPossessionEffectAction(PossessionItems pos);
}