using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MapScenesSwicher : MonoBehaviour{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
    }
    // Update is called once per frame
    void Update(){
        if (Keyboard.current.mKey.wasPressedThisFrame){
            LoadMap();
        }
    }
    public void LoadMap(){
        SceneManager.LoadScene("MiningMap");
    }
}
