using UnityEngine;
using UnityEngine.SceneManagement;
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
                    txt3.text = ThisPossession.information;
                }
                if(panel.transform.Find("Use").TryGetComponent<Button>(out Button use)){
                    use.onClick.RemoveAllListeners();
                    if(ThisPossession.effect.effectType == EffectType.none){
                        //Do nothing?
                    }
                    else if(!ThisPossession.effect.stackable && ThisPossession.effect.usedTimes >= 1){
                        //write panel and ???
                        use.onClick.AddListener(() =>{
                            if(panel.transform.Find("Msg").TryGetComponent<GameObject>(out GameObject component)){
                                if(component.transform.Find("Msg").TryGetComponent<Text>(out Text txt4)){
                                    txt4.text = "You have already used it";
                                }
                                component.SetActive(true);
                            }
                        });
                    }
                    else if(ThisPossession.effect.scene == UsableScene.All ||
                        ThisPossession.effect.scene.ToString() == SceneManager.GetActiveScene().name){
                        //create effect
                        EffectType thisEffect = pos.effect.effectType;
                        switch(thisEffect){
                            case EffectType.none:
                            case EffectType.ATKBoost:
                                EffectManager.Instance.ATKBoostFunc(pos);
                                break;
                            case EffectType.HPBoost:
                                break;
                            case EffectType.SPDUp:
                                EffectManager.Instance.SPDUpFunc(pos);
                                break;
                            case EffectType.HPUP:
                                break;
                            default:
                                break;
                        }
                        EffectManager.Instance.nowEffect.Add(pos);
                    }
                    else{ //
                        use.onClick.AddListener(() =>{
                            if(panel.transform.Find("Msg").TryGetComponent<GameObject>(out GameObject component)){
                                component.SetActive(true);
                            }
                        });
                    }
                }   
                panel.SetActive(true);
            });
        }
    }
}