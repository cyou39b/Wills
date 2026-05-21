using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 控制map的script

public class MapScenesSwicher : MonoBehaviour{
    public GameObject Map;
    public GameObject MapButtom;
    public static bool isMapOpening = false;
    int mapID;
    void Start(){}

    void Update(){
        if (Keyboard.current.mKey.wasPressedThisFrame && 
            MenuManager.IsMenuOpen == false && 
            !MenuInMining.IsMenuPanelOpening){
            LoadMap();
        }
        if(Map.activeSelf && Keyboard.current.escapeKey.wasPressedThisFrame){
            CloseMap();
        }
        if (MenuManager.IsMenuOpen || isMapOpening){
            MapButtom.SetActive(false);
        }
        else{
            MapButtom.SetActive(true);
        }
    }

    public void LoadMap(){
        Map.SetActive(true);
        isMapOpening = true;
        mapID = UIStack.Instance.NewPanel(() =>{
            isMapOpening = false;
            Map.SetActive(false);

        });
    }

    public void CloseMap()
    {
        UIStack.Instance.RemovePanel(mapID);
    }
}