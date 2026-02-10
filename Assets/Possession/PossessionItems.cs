using UnityEngine;

//Althouth the script's name is possession,the concept is that the thing you can possess
[CreateAssetMenu(fileName = "NewPossession", menuName = "Possession/items")]
public class PossessionItems : ScriptableObject{
    public int index;
    public Sprite sprite;
    public string Name;
    public string Effect;
    //public bool isPossessd = false;
    public int Num;
}