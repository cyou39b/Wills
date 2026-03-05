using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopInterfaceLogic : MonoBehaviour{
    public GameObject Panel; //buying Interface
    public GameObject Shop;
    List<Merchandise> Goods;
    List<Merchandise> SoldGoods;
    public Merchandise[] AllGoods;
    Merchandise current = null;
    public static bool isBuying = false;
    public GameObject[] AllItems;
    public Text[] ButtonText;
    public static ShopInterfaceLogic Instance{get;private set;} = null;
    public static float timeScaleBeforeShoppingStart;
    public Text MineNum;
    public GameObject Menu;
    public Button Yes;
    public Text MineNumInPanel;
    public GameObject IF;
    public GameObject SoldMsg;
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
            GlobalVariables.Instance.possession.Add(GlobalVariables.Instance.AllPossession[current.indexInPosssession]);
        }
    }
    public void CloseShop(){
        Shop.SetActive(false);
        isBuying = false;
        Menu.SetActive(true);
        //RecordMerchandise();
        Time.timeScale = timeScaleBeforeShoppingStart;
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
        Panel.SetActive(false);
    }
    public void CloseIFMsg(){
        IF.SetActive(false);
    }
    void OpenIFMsg(){
        IF.SetActive(true);
    }
    void OpenSoldMsg(){
        SoldMsg.SetActive(true);
    }
    public void CloseSoldMsg(){
        SoldMsg.SetActive(false);
    }
    /*void RecordMerchandise(){
        GlobalVariables.Instance.GoodsRecords = AllGoods;
    }*/
}