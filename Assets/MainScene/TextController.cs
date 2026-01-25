using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour{

    public Text MineText;
    void Start(){
        UpdateNumMinesText();
    }

    void Update(){
        UpdateNumMinesText();
    }
    void UpdateNumMinesText(){
        if (MenuManager.IsMenuOpen){
            MineText.enabled = false;
        }
        else{
            MineText.enabled = true;
        }
        MineText.text = $"Mine : {GlobalVariables.Instance.NumMines}";
    }
}