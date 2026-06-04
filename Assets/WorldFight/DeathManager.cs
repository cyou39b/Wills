using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeathManager : MonoBehaviour
{
    public GameObject FirstGroup;
    public GameObject SecondGroup;
    public GameObject Bkgd;

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

    private bool coroutineSpawned = false;
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
        Bkgd.SetActive(true);
        FirstGroup.SetActive(true);
        Image[] FirstGroupChildImage = FirstGroup.GetComponentsInChildren<Image>();
        Text[] FirstGroupChildText = FirstGroup.GetComponentsInChildren<Text>();
        
        SecondGroup.SetActive(true);
        Image[] SecondGroupChildImage = SecondGroup.GetComponentsInChildren<Image>();
        Text[] SecondGroupChildText = SecondGroup.GetComponentsInChildren<Text>();

        int totalFrames = (int)(slowDownTime / (1.0f / 20.0f)) + 1;
        float dAlpha = 0.5f / totalFrames;
        float dTimeScale = (Time.timeScale - targetTimeScale) / (totalFrames + 1);

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
        Time.timeScale = 0.0f; // Just to be sure that there's no fp arithmetic bs

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
        Activated = false;
        Time.timeScale = 1.0f;
        GlobalVariables.Instance.mainScenePosition = new Vector3(-20.0f, 2.5f);
        LoadSceneManager.LoadBufferAndLoadScene("MainScene");
    }

    public void QuitCallBack()
    {
        Activated = false;
        Time.timeScale = 1.0f;
        LoadSceneManager.LoadBufferAndLoadScene("MainMenu");
    }
}