using UnityEngine;
using UnityEngine.UI;

public class PossessionLogic : MonoBehaviour{
    public void Initialize(PossessionItems pos,GameObject panel){
        PossessionItems ThisPossession = pos;
        if(gameObject.transform.Find("Image").TryGetComponent<Image>(out Image img)){
            img.sprite = ThisPossession.sprite;
        }
        if(gameObject.transform.Find("Button").TryGetComponent<Button>(out Button btn)){
            btn.onClick.RemoveAllListeners();
            if(btn.gameObject.transform.Find("Text").TryGetComponent<Text>(out Text txt)){
                txt.text = ThisPossession.Name;
            }
            btn.onClick.AddListener(() =>{
                if(panel.transform.Find("Sprite").TryGetComponent<Image>(out Image img1)){
                    img1.sprite = ThisPossession.sprite;
                }
                else{
                    Debug.LogError("Couldn't find the GameObject");
                    return;
                }
                if(panel.transform.Find("Name").TryGetComponent<Text>(out Text txt1)){
                    txt1.text = ThisPossession.Name;
                }
                if(panel.transform.Find("Num").TryGetComponent<Text>(out Text txt2)){
                    txt2.text = $"Number : {ThisPossession.Num.ToString()}";
                }
                if(panel.transform.Find("Info").TryGetComponent<Text>(out Text txt3)){
                    txt3.text = ThisPossession.Effect;
                }
                panel.SetActive(true);
            });
        }
    }
}