using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MapScenesSwicher : MonoBehaviour{
    void Update(){
        if (Keyboard.current.mKey.wasPressedThisFrame){
            LoadMap();
        }
    }
    public void LoadMap(){
        SceneManager.LoadScene("MiningMap"); 
        //? The map is a scene?
        //  I think it should be something like a UI on canvas
    }
}
