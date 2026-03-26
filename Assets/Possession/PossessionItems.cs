using UnityEngine;

//Althouth the script's name is possession,the concept is that the thing you can possess
[CreateAssetMenu(fileName = "NewPossession", menuName = "Possession/items")]
public class PossessionItems : ScriptableObject{
    public int index; // the indexes start from zero
    public Sprite sprite;
    public string Name;
    [TextArea(1,5)]
    public string information;
    public int Num;
    public ItemEffect effect;
}
public enum EffectType{
    none,
    ATKBoost,
    HPBoost
}
public enum UsableScene{
    none,
    WorldFight,
    MainScene,
    WorldMining,
    All
}
[System.Serializable]
public class ItemEffect{
    public EffectType effectType;
    public float EffectRate; // 1~100%
    public UsableScene scene;
    public bool stackable;
    public int usedTimes;
}