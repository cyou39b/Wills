using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopInterfaceLogic : MonoBehaviour{
    public GameObject Panel; //buying Interface
    int pid;

    public GameObject Shop;
    int shopPid;
    List<Merchandise> Goods;
    List<Merchandise> SoldGoods;
    public Merchandise[] AllGoods;

    Merchandise current = null;
    public static bool isBuying = false;
    public GameObject[] AllItems;
    public Text[] ButtonText;

    public static ShopInterfaceLogic Instance{get;private set;} = null;
    float timeScaleBeforeShoppingStart;

    public Text MineNum;
    public GameObject Menu;
    public Button Yes;
    public Text MineNumInPanel;

    public GameObject IF;
    int IFMsgPid;
    public GameObject SoldMsg;
    int soldMsgPid;
    public PossessionItems MinesInPossession;
    void Awake(){
        if(Instance == null){
            Instance = this;
        }
        else{
            Destroy(this.gameObject);
            return;
        }
    }
    void Start(){
        //AllGoods = GlobalVariables.Instance.GoodsRecords;
        UpdateButton();
    }
    void Update(){
        UpdateMineText();
    }

    void UpdateMerchandise(){
        foreach(Merchandise items in Goods){
            if(items.num == 0){
                Goods.Remove(items);
                SoldGoods.Add(items);
            }
        }
    }
    public void SelectGoods(Merchandise goods){
        Yes.onClick.RemoveAllListeners();
        current = goods;
        Panel.SetActive(true);
        pid = UIStack.Instance.NewPanel(() =>{
            Panel.SetActive(false);
        });
        Debug.Log($"pid is {pid}");

        //write panel information
        if(Panel.transform.Find("ItemsName").TryGetComponent<Text>(out Text txt1)){
            txt1.text = current.Name;
        }
        if (Panel.transform.Find("Information").TryGetComponent<Text>(out Text txt2)){
            txt2.text = current.Information;
        }
        if(Panel.transform.Find("Picture").TryGetComponent<Image>(out Image img)){
            img.sprite = current.pic;
        }
        if(Panel.transform.Find("Price").TryGetComponent<Text>(out Text txt3)){
            txt3.text = $"Price : {current.Price.ToString()}";
        }
        if(current.num > 0 && GlobalVariables.Instance.NumMines >= current.Price){
            Yes.onClick.AddListener(BuySomething);
        }
        else if(current.Price > GlobalVariables.Instance.NumMines){
            Yes.onClick.AddListener(OpenIFMsg);
        }
        else if(current.num <= 0){
            Yes.onClick.AddListener(OpenSoldMsg);
        }
    }

    void BuySomething(){
        if(current.Price<= GlobalVariables.Instance.NumMines){
            current.num -= 1;
            GlobalVariables.Instance.NumMines -= current.Price;
            MinesInPossession.Num = GlobalVariables.Instance.NumMines;
            // GlobalVariables.Instance.possession.Add(GlobalVariables.Instance.AllPossession[current.indexInPosssession]);
            GlobalVariables.Instance.AllPossession[current.indexInPosssession].Num++;
        }
    }
    public void CloseShop(){
        UIStack.Instance.RemovePanel(shopPid);
    }

    void UpdateButton(){
        for(int i = 0; i< ButtonText.Length ; i++){
            if (ButtonText.Length != AllGoods.Length){
                Debug.LogError("Couldn't write text correctly");
                break;
            }
            ButtonText[i].text = AllGoods[i].Name;
            if(AllItems[i].transform.Find("Image").TryGetComponent<Image>(out Image img)){
                img.sprite = AllGoods[i].pic;
            }
        }
    }
    void UpdateMineText(){
        if (Panel.activeSelf){
            MineNumInPanel.text = $"Mine : {GlobalVariables.Instance.NumMines}";
        }
        MineNum.text = $"Mine : {GlobalVariables.Instance.NumMines}";
    }
    public void ClosePanel(){
        UIStack.Instance.RemovePanel(pid);
    }
    public void CloseIFMsg(){
        UIStack.Instance.RemovePanel(IFMsgPid);
    }
    void OpenIFMsg(){
        IFMsgPid = UIStack.Instance.NewPanel(() =>{
            IF.SetActive(false);
        });

        IF.SetActive(true);
    }

    void OpenSoldMsg(){
        soldMsgPid = UIStack.Instance.NewPanel(() =>{
            SoldMsg.SetActive(false);
        });
        SoldMsg.SetActive(true);
    }
    public void CloseSoldMsg(){
        UIStack.Instance.RemovePanel(soldMsgPid);
    }
    /*void RecordMerchandise(){
        GlobalVariables.Instance.GoodsRecords = AllGoods;
    }*/
    public void OpenShop(){
        shopPid = UIStack.Instance.NewPanel(() =>{
            Shop.SetActive(false);
            isBuying = false;
            Menu.SetActive(true);
            //RecordMerchandise();
            Time.timeScale = timeScaleBeforeShoppingStart;
            MenuInSceneLogic.HaveToCloseInteractButton = false;
        });

        Shop.SetActive(true);
        DialogueManager.Instance.EndDialogue();
        Menu.SetActive(false);
        isBuying = true;
        timeScaleBeforeShoppingStart = Time.timeScale;
        Time.timeScale = 0.0f;
        MenuInSceneLogic.HaveToCloseInteractButton = true;
    }
}