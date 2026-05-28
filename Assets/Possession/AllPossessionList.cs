using UnityEngine;

public class AllPossessionList : ScriptableObject
{
    public PossessionItems[] List;
    public void Load(PlayerData data)
    {
        foreach(PlayerData.PossessionPair item in data.AllPossessions)
        {
            foreach(PossessionItems pItem in List)
            {
                if(pItem.index == item.index)
                {
                    pItem.Num = item.num;
                    break;
                }
            }
        }
    }
}