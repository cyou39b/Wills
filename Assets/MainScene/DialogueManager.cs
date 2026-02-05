using UnityEngine;
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
    int index = 0;
    public Button Option1;
    public Button Option2;
    public GameObject Shop;
    void Awake(){
        if(Instance == null){
            Instance = this;
        }
        else{
            Destroy(this.gameObject);
            return;
        }
    }
    void Update(){}
    public void StartDialogue(IInteract interact){
        IsTalking = true;
        Menu.SetActive(false);
        timeScaleBeforeDialogeStart = Time.timeScale;
        Time.timeScale = 0.0f;
        index = 0;
        current = interact.GetDialogueData();
        Dialogue.text = current.lines[index].content;
        Speaker.text = current.lines[index].speaker;
        DialoguePanel.SetActive(true);
    }
    public void NextLine(){
        Option1.gameObject.SetActive(false);
        Option2.gameObject.SetActive(false);

        GoNextLine.gameObject.SetActive(true);
        
        Option1.onClick.RemoveAllListeners();
        Option2.onClick.RemoveAllListeners();

        index++;
        if(index >= current.lines.Count){
            EndDialogue();
            return;
        }
        else{
            Dialogue.text = current.lines[index].content;
            Speaker.text = current.lines[index].speaker;
        }
        if(!string.IsNullOrEmpty(current.lines[index].optionText1)){
            if(Option1.transform.Find("Text1").TryGetComponent<Text>(out Text txt1)){
                txt1.text = current.lines[index].optionText1;
                Option1.gameObject.SetActive(true);
                Option1.onClick.AddListener(() =>
                ExecuteCommand(current.lines[index].option1Command));
            }
            if(!string.IsNullOrEmpty(current.lines[index].optionText2)){
                if(Option2.transform.Find("Text2").TryGetComponent<Text>(out Text txt2)){
                    txt2.text = current.lines[index].optionText2;
                    Option2.gameObject.SetActive(true);
                    Option2.onClick.AddListener(() =>
                    ExecuteCommand(current.lines[index].option2Command));
                }
            }
            GoNextLine.gameObject.SetActive(false);
        }
    }
    public void EndDialogue(){
        DialoguePanel.SetActive(false);
        IsTalking = false;
        Menu.SetActive(true);
        Time.timeScale = timeScaleBeforeDialogeStart;
        current = null;
    }
    void ExecuteCommand(DialogueCommand cmd){
        switch (cmd){
            case DialogueCommand.none:
                NextLine();
                break;
            case DialogueCommand.End:
                EndDialogue();
                break;
            case DialogueCommand.OpenShop:
                Shop.SetActive(true);
                EndDialogue();
                break;
        }
    }
}