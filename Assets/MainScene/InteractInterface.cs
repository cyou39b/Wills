using UnityEngine;

public interface IInteract{
    string WriteInteractText();
    void OnButtonClick();
    Vector2 GetPosition();
    DialogueData GetDialogueData();
}