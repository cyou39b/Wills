using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class BackpackLogic : MonoBehaviour{
    public GameObject Backpack;
    public GameObject Panel;
    public GameObject Prefab;
    List<GameObject> BackpackGameobjectButtonList = new List<GameObject>();
    public GameObject Content;
    public Button BackpackIcon;
    public GameObject ISMsg;
    public static bool IsBackpackOpening = false;
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
        BackpackIcon.gameObject.SetActive(true);
        IsBackpackOpening = false;
        foreach(GameObject items in BackpackGameobjectButtonList){
            Destroy(items);
        }
    }
    public void Spawn(){ //call when the backpack button onClick
        for(int i = 0; i < GlobalVariables.Instance.possession.Count ; i++){
            GameObject tmp = Instantiate(Prefab,Content.transform);
            if(tmp.TryGetComponent<RectTransform>(out RectTransform rect)){
                rect.anchoredPosition = new Vector2(-750+350*(i%5),0-350*math.floor(i/5));
            }
            if(tmp.TryGetComponent<PossessionLogic>(out PossessionLogic LogicScript)){
                LogicScript.Initialize(GlobalVariables.Instance.possession[i],Panel);
            }
            else{
                Debug.LogError("Get Script Failed");
                CloseBackpack();
                return;
            }
            BackpackGameobjectButtonList.Add(tmp);
        }
    }
    public void OpenBackpack(){
        BackpackIcon.gameObject.SetActive(false);
        Backpack.SetActive(true);
        IsBackpackOpening = true;
        Spawn();
    }
    public void ClosePanel(){
        Panel.SetActive(false);
    }
    public void CloseMsg(){
        ISMsg.SetActive(false);
    }
}