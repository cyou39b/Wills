using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathManager : MonoBehaviour
{
    public GameObject FirstGroup;
    public GameObject SecondGroup;

    public static bool Activated{get; private set;} = false;
    private static float targetTimeScale;
    private static float slowDownTime;
    public static void StartDeath(float targetScale, float slowTime)
    {
        if(Activated){return;}
        Activated = true;
        targetTimeScale = targetScale;
        slowDownTime = slowTime;
    }

    private static bool coroutineSpawned = false;
    void Update()
    {
        if(Activated && !coroutineSpawned)
        {
            coroutineSpawned = true;
            StartCoroutine(DeathScreen());
        }
    }

    private static readonly WaitForSecondsRealtime w20thRealSecond = new WaitForSecondsRealtime(1.0f / 20.0f);
    public IEnumerator DeathScreen()
    {
        FirstGroup.SetActive(true);
        Image[] FirstGroupChildImage = GetComponentsInChildren<Image>();
        Text[] FirstGroupChildText = GetComponentsInChildren<Text>();
        
        SecondGroup.SetActive(true);
        Image[] SecondGroupChildImage = GetComponentsInChildren<Image>();
        Text[] SecondGroupChildText = GetComponentsInChildren<Text>();

        int totalFrames = (int)(slowDownTime / (1.0f / 20.0f)) + 1;
        float dAlpha = 0.5f / totalFrames;
        float dTimeScale = (Time.timeScale - targetTimeScale) / totalFrames;

        for(int _=0;_<totalFrames;_++)
        {
            Time.timeScale -= dTimeScale;
            foreach(Image child in FirstGroupChildImage)
            {
                Color c = child.color;
                c.a += dAlpha;
                child.color = c;
            }
            foreach(Text child in FirstGroupChildText)
            {
                Color c = child.color;
                c.a += dAlpha;
                child.color = c;
            }
            yield return w20thRealSecond;
        }

        totalFrames = 12;
        dAlpha = 1.0f/totalFrames;
        for(int _=0;_<totalFrames;_++)
        {
            foreach(Image child in SecondGroupChildImage)
            {
                Color c = child.color;
                c.a += dAlpha;
                child.color = c;
            }
            foreach(Text child in SecondGroupChildText)
            {
                Color c = child.color;
                c.a += dAlpha;
                child.color = c;
            }
            yield return w20thRealSecond;
        }

    }

    public void RestartCallBack()
    {
    
    }

    public void QuitCallBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}