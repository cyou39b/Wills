using UnityEngine;
using UnityEngine.UI;

public class BackpackLogic : MonoBehaviour{
    public GameObject Backpack;
    void Start(){}

    void Update(){}
    public void CloseBackpack(){
        Backpack.SetActive(false);
    }
}
