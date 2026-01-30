using UnityEngine;

public class ClerkLogic : MonoBehaviour,IInteract{
    [SerializeField]DialogueData talkToClerk;
    void Start(){}
    void Update(){}
    public string WriteInteractText() => "Clerk";
    public void OnButtonClick(){
        Debug.Log("Button Onclick");
        DialogueManager.Instance.StartDialogue(this);
    }
    public Vector2 GetPosition() => transform.position;
    public DialogueData GetDialogueData() => talkToClerk;
}