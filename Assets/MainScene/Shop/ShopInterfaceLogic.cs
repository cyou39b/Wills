using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopInterfaceLogic : MonoBehaviour{
    public GameObject Panel; //buying Interface
    public GameObject Shop;
    public List<Merchandise> Goods;
    public List<Merchandise> SoldGoods;
    public Merchandise[] AllGoods;
    public Merchandise current = null;
    public static bool isBuying = false;
    public Button[] buttons;
    public Text[] ButtonText;
    void Start(){}
    void Update(){}
    
    void UpdateMerchandise(){
        foreach(Merchandise items in Goods){
            if(items.num == 0){
                Goods.Remove(items);
                SoldGoods.Add(items);
            }
        }
    }
    public void SelectGoods(){ //change current state

    }
    public void BuySomething(){}
    public void CloseShop(){
        Shop.SetActive(false);
        isBuying = false;       
    }
    void UpdateButtonText(){
        for(int i = 0; i< ButtonText.Length ; i++){
            if (ButtonText.Length != AllGoods.Length){
                Debug.LogError("Couldn't write text correctly");
                break;
            }
            ButtonText[i].text = AllGoods[i].name;
        }
    }
}