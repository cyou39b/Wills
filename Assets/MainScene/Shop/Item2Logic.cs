using UnityEngine;

public class Item2Logic : MonoBehaviour,IShopButtonInterface{
    public Merchandise Item2;
    public Merchandise  GetMerchandise() => Item2;
    public void OnButtonClick(){
        ShopInterfaceLogic.Instance.SelectGoods(GetMerchandise());  
    }
}
