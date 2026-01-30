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
}