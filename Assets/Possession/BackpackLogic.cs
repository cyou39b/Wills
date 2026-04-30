using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class BackpackLogic : MonoBehaviour{
    public GameObject Backpack;
    public Button BackpackIcon;

    public GameObject PossessionPrefab;
    public GameObject ScrollViewContent;

    public GameObject PossessionErrorPanel;
    public Text PossessionErrorText;

    // --------- Panel Data ---------
    private PanelData panelData;
    public GameObject Panel;
    public Image PanelItemImage;
    public Text PanelItemNameText;
    public Text PanelItemNumText;
    public Text PanelInfoText;
    public Button PanelUseButton;

    private Dictionary<PossessionItems, GameObject> backpackContentGameobjects = new Dictionary<PossessionItems, GameObject>();

    public static bool IsBackpackOpening = false;

    private AudioSource openBackpackSFX;
    void Start()
    {
        openBackpackSFX = GetComponent<AudioSource>();

        panelData = new PanelData()
        {
            gameObject = Panel,
            ItemImage = PanelItemImage,
            ItemNameText = PanelItemNameText,
            ItemNumText = PanelItemNumText,
            InfoText = PanelInfoText,
            UseButton = PanelUseButton,
        };
    }

    void SpawnBackpackScrollViewItems(){ //call when the backpack button onClick
        Dictionary<PossessionItems, GameObject> newbackpackObjects = new Dictionary<PossessionItems, GameObject>();

        for(int i = 0; i < GlobalVariables.Instance.possession.Count ; i++){
            if(backpackContentGameobjects.ContainsKey(GlobalVariables.Instance.possession[i]))
            {
                newbackpackObjects.Add(GlobalVariables.Instance.possession[i], backpackContentGameobjects[GlobalVariables.Instance.possession[i]]);
                continue;
            }

            GameObject newObj = Instantiate(PossessionPrefab, ScrollViewContent.transform);

            if(!newObj.TryGetComponent<PossessionLogic>(out PossessionLogic LogicScript)){
                Debug.LogError("Get Script Failed");
                CloseBackpack();
                return;
            }

            LogicScript.Initialize(GlobalVariables.Instance.possession[i], panelData, this);
            newbackpackObjects.Add(GlobalVariables.Instance.possession[i], newObj);
        }
        backpackContentGameobjects = newbackpackObjects;
    }

    private int backpackPid;
    public void OpenBackpack(){
        IsBackpackOpening = true;

        Backpack.SetActive(true);
        BackpackIcon.gameObject.SetActive(false);

        SpawnBackpackScrollViewItems();

        openBackpackSFX.PlayOneShot(openBackpackSFX.clip);

        backpackPid = UIStack.Instance.NewPanel(
            () =>
            {
                IsBackpackOpening = false;

                Backpack.SetActive(false);
                BackpackIcon.gameObject.SetActive(true);

                ClosePanel(); // The panel might be opened, so we close it.
            }
        );
    }

    public void CloseBackpack()
    {
        UIStack.Instance.RemovePanel(backpackPid);
    }

    [NonSerialized] public int panelPid;
    public void ClosePanel(){
        UIStack.Instance.RemovePanel(panelPid);
    }

    private int errorPid;
    public void OpenPossessionErrorPanel(string errorMsg)
    {
        PossessionErrorText.text = errorMsg;
        PossessionErrorPanel.SetActive(true);
        errorPid = UIStack.Instance.NewPanel(
            () =>
            {
                PossessionErrorPanel.SetActive(false);
            }
        );
    }

    public void ClosePossessionErrorPanel()
    {
        UIStack.Instance.RemovePanel(errorPid);
    }
}

public class PanelData
{
    public GameObject gameObject;
    public Image ItemImage;
    public Text ItemNameText;
    public Text ItemNumText;
    public Text InfoText;
    public Button UseButton;
}