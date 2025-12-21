using UnityEngine;
using UnityEngine.InputSystem;

public class MenuShutdown : MonoBehaviour{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Menu;
    void Start(){   
    }

    // Update is called once per frame
    void Update(){
        if (Menu.activeSelf && Keyboard.current.escapeKey.wasPressedThisFrame){
            CloseMenu();
        }
    }
    public void CloseMenu(){
        Menu.SetActive(false);
    }
}
