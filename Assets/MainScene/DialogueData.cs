using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject{
    public List<DialogueLine> lines;
}

[System.Serializable]
public class DialogueLine{ 
    public string speaker;
    [TextArea(1,5)]
    public string content;
    public string optionText1;
    public DialogueCommand option1Command;
    public int JumpToWhichLine1; //starts from zero 

    public string optionText2;
    public DialogueCommand option2Command;
    public int JumpToWhichLine2;
}
public enum DialogueCommand{
    none,
    OpenShop,
    End,
    Fight,
    JumpToLine
}