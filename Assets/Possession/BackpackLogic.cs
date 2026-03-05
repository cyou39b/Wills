using UnityEngine;
using UnityEngine.UI;

public class BackpackLogic : MonoBehaviour{
    public GameObject Backpack;
    public GameObject Panel;
    public static BackpackLogic Instance{get;private set;} = null;
    void Awake(){
        if(Instance == null){
            Instance = this;
        }
        else{
            Destroy(this.gameObject);
            return;
        }
    }
    void Start(){}

    void Update(){}
    public void CloseBackpack(){
        Backpack.SetActive(false);
    }
}
