using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour{
    AsyncOperation async = null;
    public static string NextScene;
    void Start(){
        StartCoroutine(LoadScene());
    }
    void Update(){
        if(NextScene == null){
            #if UNITY_EDITOR
                EditorApplication.isPlaying = false; 
            #else
                Application.Quit();
            #endif
        }
    }
    // No, I still don't get this.
    // The things you're doing in Update and LoadScene can be merged into one function.
    IEnumerator LoadScene(){

        if(!string.IsNullOrEmpty(NextScene)){
            // Why async when you're doing literally NOTHING here.
            async = SceneManager.LoadSceneAsync(NextScene);
            async.allowSceneActivation = false; // To disable automatic loading
            while(!async.isDone){
                if(async.progress >= 0.9f){ // if allowSceneActivation == false , the max value of progress == 0.9
                    async.allowSceneActivation = true;
                }
                yield return null;
            }
        }
        yield return new WaitForSeconds(1.0f);
    }
}