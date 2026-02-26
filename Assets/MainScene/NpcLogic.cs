using UnityEngine;

public class NpcLogic : MonoBehaviour,IInteract{
    [SerializeField]DialogueData talkToNPC;
    void Start(){}
    void Update(){}

    public string WriteInteractText() => "NPC";
    public void OnButtonClick(){
        DialogueManager.Instance.StartDialogue(this);
    }
    public Vector2 GetPosition() => transform.position;
    public DialogueData GetDialogueData() => talkToNPC; 
}