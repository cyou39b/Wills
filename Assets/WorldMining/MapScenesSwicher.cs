using UnityEngine;
using UnityEngine.InputSystem;

public class MapScenesSwicher : MonoBehaviour{
    public GameObject Map;
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
    }

    public void CloseMap(){
        Map.SetActive(false);
    }

}