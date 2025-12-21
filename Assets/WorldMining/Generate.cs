using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Generate : MonoBehaviour{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform JackObj;
    public GameObject Model;
    public List<GameObject> CreateModel;
    public double minDis = 0; 
    double oldDis =  0 ; //record the old value to compare 
    void Start(){
        RandomObj();
    }
    // Update is called once per frame
    void Update(){
        if (Keyboard.current.shiftKey.wasPressedThisFrame){
            ResetObj();
            RandomObj();
        }
        CalculateDistance();        
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
    }
    void CalculateDistance(){
        foreach(GameObject items in CreateModel){
            if(items == null){
                continue;
            }
            double xdis = (items.transform.position.x - JackObj.position.x) * (items.transform.position.x - JackObj.position.x);
            double ydis = (items.transform.position.y - JackObj.position.y) * (items.transform.position.y - JackObj.position.y);
            double dis = xdis + ydis; 
            //TODO
            if (dis < oldDis){ 
                minDis = dis;
            }
            if ((float)dis < 1f && items != null && Keyboard.current[GlobalVariables.Instance.FindMine].wasPressedThisFrame){
                Destroy(items);
            }
            oldDis = dis;
        }
    }
}