using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 控制map的script

public class MapScenesSwicher : MonoBehaviour{
    public GameObject Map;
    public Text numMine;
    public static bool isMapOpening = false;
    void Start(){}

    void Update(){
        if (Keyboard.current.mKey.wasPressedThisFrame){
            LoadMap();
        }
        if(Map.activeSelf && Keyboard.current.escapeKey.wasPressedThisFrame){
            CloseMap();
        }
    }

    public void LoadMap(){
        Map.SetActive(true);
        numMine.enabled = false;
        isMapOpening = true;
    }

    public void CloseMap(){
        isMapOpening = false;
        Map.SetActive(false);
        numMine.enabled = true;
    }
}