using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//shit
public class PossessionLogic : MonoBehaviour{
    public Image itemImg;
    public Button itemButton;
    public Text itemButtonName;

    private PanelData Panel;
    private PossessionItems Possession;
    private BackpackLogic Backpack;

    public void Initialize(PossessionItems poss, PanelData pd, BackpackLogic bp){
        // NOTE: For other functions in this GameObject
        Possession = poss;
        Panel = pd;
        Backpack = bp;
        
        itemImg.sprite = Possession.sprite;
        itemButtonName.text = Possession.Name;

        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(OpenPanelForPossession);
    }

    void OpenPanelForPossession()
    {
        Panel.gameObject.SetActive(true);

        Panel.ItemImage.sprite = Possession.sprite;

        Panel.ItemNameText.text = Possession.Name;
        Panel.ItemNumText.text = $"Number: {Possession.Num}";
        Panel.InfoText.text = Possession.information;

        Panel.UseButton.onClick.RemoveAllListeners();
        Panel.UseButton.onClick.AddListener(GetThisPossessionOnUseAction());

        Backpack.panelPid = UIStack.Instance.NewPanel(
            () =>
            {
                Panel.gameObject.SetActive(false);
            }
        );
    }

    UnityEngine.Events.UnityAction GetThisPossessionOnUseAction()
    {
        if(Possession.effect.effectType == EffectType.none || Possession.Num <= 0)
        {
            return () => {};
        }
        else if(!Possession.effect.stackable && Possession.effect.usedTimes >= 1) // if tihs effect is not stackable and it's already in use
        {
            return () => {Backpack.OpenPossessionErrorPanel("Nonstackable!");};
        }
        else if(Possession.effect.scene == UsableScene.All || Possession.effect.scene.ToString() == SceneManager.GetActiveScene().name)
        {
            return () =>
                {
                    Debug.Log($"{Possession.Name} is used");

                    Possession.effect.usedTimes++;
                    Possession.Num--;

                    EffectManager.Instance.TrueInstance.GetPossessionEffectAction(Possession).Invoke();

                    Backpack.CloseBackpack();
                };
        }
        return () => {Backpack.OpenPossessionErrorPanel("UIAEBFLAKBASEHLFUISF<");};
    }
}