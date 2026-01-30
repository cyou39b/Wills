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
        index++;
        if(index >= current.lines.Count){
            EndDialogue();
            return;
        }
        else{
            Dialogue.text = current.lines[index].content;
            Speaker.text = current.lines[index].speaker;
        }
    }
    public void EndDialogue(){
        DialoguePanel.SetActive(false);
        IsTalking = false;
        Menu.SetActive(true);
        Time.timeScale = timeScaleBeforeDialogeStart;
        current = null;
    }
}