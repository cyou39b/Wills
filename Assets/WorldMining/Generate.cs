using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//! WARN: Deprecated file, moved to Machine Logic
public class Generate : MonoBehaviour{
    public Transform JackTransform;
    public GameObject Model;
    public List<GameObject> CreatedModel;
    public Text scoreText;
    void Start(){
        RandomObj();
        UpdateNumMinesText();
    }
    void Update(){
        if (Keyboard.current.shiftKey.wasPressedThisFrame){
            ResetObj();
            RandomObj();
        }
        UpdateNumMinesText();  
    }
    void RandomObj(){
        for(int i=0;i<10;i++){
            float setAngle = Random.Range(-Mathf.PI,Mathf.PI);
            float setDistant = Random.Range(5f,20f);
            float offsetx = Mathf.Cos(setAngle) * setDistant;
            float offsety = Mathf.Sin(setAngle) * setDistant;
            Vector2 newpos = new Vector2(JackTransform.position.x + offsetx, JackTransform.position.y + offsety);
            CreatedModel.Add(Instantiate(Model,newpos,transform.rotation));
        }
    }
    void ResetObj(){
        for(int i = 0; i < CreatedModel.Count ;i++){
            Destroy(CreatedModel[i]);
        }
        CreatedModel.Clear();
    }
    void UpdateNumMinesText(){
        scoreText.text = $"Mine: {GlobalVariables.Instance.NumMines}";
    }
}