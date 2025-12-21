using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MapScenesSwicher : MonoBehaviour{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Map;
    void Start(){
    }
    // Update is called once per frame
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