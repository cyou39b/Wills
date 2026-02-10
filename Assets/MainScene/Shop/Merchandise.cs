using UnityEngine;

[CreateAssetMenu(fileName = "Merchandise", menuName = "Merchandise/Goods")]
public class Merchandise : ScriptableObject{
    public int index;
    public string Name;
    public Sprite pic;
    public string Information;
    public int Price;
    public int indexInPosssession;
    //public bool isBought = false;
    public int num;
}