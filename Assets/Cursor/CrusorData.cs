using UnityEngine;

[CreateAssetMenu(fileName = "CrusorData", menuName = "Scriptable Objects/CrusorData")]
public class CrusorData : ScriptableObject
{
    public string[] Names;
    public Texture2D[] Textures;
}