using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 控制map的script

public class MapScenesSwicher : MonoBehaviour{
    public GameObject Map;
    public GameObject MapButtom;
    public static bool isMapOpening = false;
    void Start(){}

    void Update(){
        if (Keyboard.current.mKey.wasPressedThisFrame &&  MenuManager.IsMenuOpen == false){
            LoadMap();
        }
        if(Map.activeSelf && Keyboard.current.escapeKey.wasPressedThisFrame){
            CloseMap();
        }
        if (MenuManager.IsMenuOpen){
            MapButtom.SetActive(false);
        }
        else{
            MapButtom.SetActive(true);
        }
    }

    public void LoadMap(){
        Map.SetActive(true);
        isMapOpening = true;
    }

    public void CloseMap(){
        isMapOpening = false;
        Map.SetActive(false);
    }
}