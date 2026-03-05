using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackLogic : MonoBehaviour{
    public GameObject Backpack;
    public GameObject Panel;
    public GameObject Prefab;
    List<GameObject> BackpackGameobjectButtonList;
    public Canvas canva;
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
    void Spawn(){
        //foreach(PossessionItems item in GlobalVariables.Instance.possession){}
        for(int i = 0; i < GlobalVariables.Instance.possession.Count ; i++){
            GameObject tmp = Instantiate(Prefab,canva.transform);
            BackpackGameobjectButtonList.Add(tmp);
        }
    }
}
