using UnityEngine;
using UnityEditor;

public class MiningMenuLogic : MonoBehaviour{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Menu;
    void Start(){
    }
    // Update is called once per frame
    void Update(){    
    }
    public void OpenMenu(){
        Menu.SetActive(true);
    }
}