using System;
using System.Collections.Generic;
using UnityEngine;

public class ClerkLogic : MonoBehaviour,IInteract{
    [SerializeField]DialogueData talkToClerk;
    public GameObject Shop;
    void Start(){
        ClerkOption1.Add(1,Line1OnOption1Click);
    }
    void Update(){}
    public string WriteInteractText() => "Clerk";
    public void OnButtonClick(){
        Debug.Log("Button Onclick");
        DialogueManager.Instance.StartDialogue(this);
    }
    public Vector2 GetPosition() => transform.position;
    public DialogueData GetDialogueData() => talkToClerk;
    public void Line1OnOption1Click(){
        Shop.SetActive(true);
        DialogueManager.Instance.EndDialogue();
    }
    public static Dictionary<int,Action> ClerkOption1 = new Dictionary<int, Action>{};
    public static Dictionary<int,Action> ClerkOption2 = new Dictionary<int, Action>{};
}