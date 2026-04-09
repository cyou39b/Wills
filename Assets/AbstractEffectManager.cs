using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractEffectManager : MonoBehaviour{
    public virtual void ATKBoostFunc(PossessionItems pos){}
    public abstract void SPDUpFunc(PossessionItems pos);
    public virtual void HPBoostFunc(PossessionItems pos){}
    public virtual void HPUPFunc(PossessionItems pos){}
    public abstract void ClearEffect();
}