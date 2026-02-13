using UnityEngine;

public class Items1Logic : MonoBehaviour,IShopButtonInterface{
    public Merchandise Items1;
    void Start(){}

    void Update(){}
    public Merchandise GetMerchandise(){
        return Items1;
    }
    public void OnButtonClick(){
        ShopInterfaceLogic.Instance.SelectGoods(GetMerchandise());
    }
}