using UnityEngine;

public class MachineLogic : MonoBehaviour{
    public AudioSource findMine;
    void Start(){
    }
    // Update is called once per frame
    void Update(){
        if (FindMine.IsDetect){
            findMine.Play();
        }
    }
}
