using UnityEngine;

public class ClerkLogic : MonoBehaviour,IInteract{
    [SerializeField]DialogueData talkToClerk;
    public GameObject Shop;
    void Start(){}
    void Update(){}
    public string WriteInteractText() => "Clerk";
    public void OnButtonClick(){
        DialogueManager.Instance.StartDialogue(this);
    }
    public Vector2 GetPosition() => transform.position;
    public DialogueData GetDialogueData() => talkToClerk;
}