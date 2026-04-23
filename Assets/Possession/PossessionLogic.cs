using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//shit
public class PossessionLogic : MonoBehaviour{
    public void Initialize(PossessionItems pos,GameObject panel,BackpackLogic logic){
        PossessionItems ThisPossession = pos;
        string nowSceneName = SceneManager.GetActiveScene().name;
        //Transform effectManager = transform.Find("EffectManager");
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
                    txt3.text = ThisPossession.information;
                }
                if(panel.transform.Find("Use").TryGetComponent<Button>(out Button use)){
                    use.onClick.RemoveAllListeners();
                    if(ThisPossession.effect.effectType == EffectType.none || ThisPossession.Num <= 0){
                        //Do nothing?
                    }
                    else if(!ThisPossession.effect.stackable && ThisPossession.effect.usedTimes >= 1){
                        //write panel and ???
                        use.onClick.AddListener(() =>{
                            Transform msg = panel.transform.Find("Msg");
                            if(msg != null){
                                msg.gameObject.SetActive(true);
                            }
                        });
                    }
                    else if(ThisPossession.effect.scene == UsableScene.All ||
                            ThisPossession.effect.scene.ToString() == nowSceneName){
                        use.onClick.AddListener(() =>{
                            ThisPossession.effect.usedTimes++;
                            //create effect
                            ThisPossession.Num--;
                            Action function = EffectManager.Instance.TrueInstance.GetPossessionEffectAction(ThisPossession);
                            function();
                            Debug.Log($"{pos.Name} is used");
                            if(ThisPossession.Num <= 0){
                                GlobalVariables.Instance.possession.Remove(ThisPossession);
                            }
                            logic.CloseBackpack();
                            //logic.OpenBackpack();
                            });
                    }
                    else{ //
                        use.onClick.AddListener(() =>{
                            Transform Msg = panel.transform.Find("Msg");
                            if(Msg != null){
                                Msg.gameObject.SetActive(true);
                            }
                        });
                    }
                }   
                panel.SetActive(true);
            });
        }
    }
}