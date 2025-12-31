using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Generate : MonoBehaviour{
    public Transform JackObj;
    public GameObject Model;
    public List<GameObject> CreateModel;
    public static int Score1 = 0; // I hope it won't explode
    public Text scoreText;
    int resetTimes = 3;
    public Text resetMsg;
    void Start(){
        RandomObj();
        SetScore();
        SetResmsg();
    }
    void Update(){
        if (Keyboard.current.shiftKey.wasPressedThisFrame && resetTimes > 0){
            ResetObj();
            RandomObj();
        }
        SetScore();  
        SetResmsg();
    }
    void RandomObj(){
        for(int i=0;i<10;i++){
            float setAngle = Random.Range(-Mathf.PI,Mathf.PI);
            float setDistant = Random.Range(5f,20f);
            float offsetx = Mathf.Cos(setAngle) * setDistant;
            float offsety = Mathf.Sin(setAngle) * setDistant;
            Vector2 newpos = new Vector2(JackObj.position.x + offsetx, JackObj.position.y + offsety);
            CreateModel.Add(Instantiate(Model,newpos,transform.rotation));
        }
    }
    void ResetObj(){
        for(int i = 0; i < CreateModel.Count ;i++){
            Destroy(CreateModel[i]);
        }
        CreateModel.Clear();
        Score1 = 0;
        resetTimes -= 1;
    }
    void SetScore(){
        scoreText.text = $"Score : {Score1}";
    }
    void SetResmsg(){
        resetMsg.text = $"Reset : {resetTimes}";
    }
}