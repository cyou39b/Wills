using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour{
    public GameObject DialoguePanel;
    public static bool IsTalking = false;
    float timeScaleBeforeDialogeStart;
    public Text Dialogue;
    public Text Speaker;
    public Button GoNextLine;
    public static DialogueManager Instance{get;private set;}
    DialogueData current = null;
    public GameObject Menu;
    public GameObject MenuInSceneButton;
    int index = 0;
    public Button Option1;
    public Button Option2;
    void Awake(){
        if(Instance == null){
            Instance = this;
        }
        else{
            Destroy(this.gameObject);
            return;
        }
    }
    public void StartDialogue(IInteract interact){
        if (MenuManager.IsMenuOpen || 
            ShopInterfaceLogic.isBuying || 
            BackpackLogic.IsBackpackOpening){
            return;
        }
        Option1.gameObject.SetActive(false);
        Option2.gameObject.SetActive(false);
        GoNextLine.gameObject.SetActive(true);

        IsTalking = true;
        MenuInSceneButton.SetActive(false);
        Menu.SetActive(false);

        timeScaleBeforeDialogeStart = Time.timeScale;
        Time.timeScale = 0.0f;
        index = 0;
        current = interact.GetDialogueData();
        
        //Dialogue.text = current.lines[index].content;
        //Speaker.text = current.lines[index].speaker;
        NextLine();
        DialoguePanel.SetActive(true);
    }
    public void NextLine(){
        Option1.gameObject.SetActive(false);
        Option2.gameObject.SetActive(false);
        GoNextLine.gameObject.SetActive(true);

        Option1.onClick.RemoveAllListeners();
        Option2.onClick.RemoveAllListeners();
        
        if(index >= current.lines.Count){
            EndDialogue();
            return;
        }
        else{
            Dialogue.text = current.lines[index].content;
            Speaker.text = current.lines[index].speaker;
        }
        if(!string.IsNullOrEmpty(current.lines[index].optionText1)){
            int optionIndex = index;

            if(Option1.transform.Find("Text1").TryGetComponent<Text>(out Text txt1)){
                txt1.text = current.lines[index].optionText1;
                Option1.gameObject.SetActive(true);
                Option1.onClick.AddListener(() =>
                ExecuteCommand(current.lines[optionIndex].option1Command,1));
            }

            if(!string.IsNullOrEmpty(current.lines[index].optionText2)){
                if(Option2.transform.Find("Text2").TryGetComponent<Text>(out Text txt2)){
                    txt2.text = current.lines[index].optionText2;
                    Option2.gameObject.SetActive(true);
                    Option2.onClick.AddListener(() =>
                    ExecuteCommand(current.lines[optionIndex].option2Command,2));
                }
            }
            GoNextLine.gameObject.SetActive(false);
        }
        index++;
    }
    public void EndDialogue(){
        DialoguePanel.SetActive(false);
        IsTalking = false;
        Menu.SetActive(true);
        MenuInSceneButton.SetActive(true);
        Time.timeScale = timeScaleBeforeDialogeStart;
        current = null;
    }
    void ExecuteCommand(DialogueCommand cmd,int optionNumber){
        switch (cmd){
            case DialogueCommand.none:
                NextLine();
                break;
            case DialogueCommand.End:
                EndDialogue();
                index = 0;
                break;
            case DialogueCommand.OpenShop:
                ShopInterfaceLogic.Instance.OpenShop();
                index = 0;
                break;
            case DialogueCommand.Fight:
                EndDialogue();
                EnemySpawner.spawnCnt = 5;
                LoadSceneManager.NextScene = "WorldFight";
                SceneManager.LoadScene("LoadSceneBuffer");
                break;
            case DialogueCommand.JumpToLine:
                switch (optionNumber){
                    case 1:
                        index = current.lines[index].JumpToWhichLine1;
                        break;
                    case 2:
                        index = current.lines[index].JumpToWhichLine2;
                        break;
                    default:
                        Debug.LogError("You give a invalid number");
                        break;
                }
                NextLine();
                break;
        }
    }
}