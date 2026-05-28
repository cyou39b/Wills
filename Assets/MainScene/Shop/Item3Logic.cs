using UnityEngine;

public class Item3Logic : MonoBehaviour,IShopButtonInterface{
    public Merchandise Item3;
    public Merchandise GetMerchandise() => Item3;
    public void OnButtonClick(){
        ShopInterfaceLogic.Instance.SelectGoods(GetMerchandise());
    }
}
