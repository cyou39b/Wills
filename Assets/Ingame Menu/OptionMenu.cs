using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

// menu中options的部分
public class OptionMenu : MonoBehaviour
{
    public static bool IsBinding = false;
    public GameObject BindingBlur; // 在設定按鍵時避免玩家同時設定到別的按鍵的GameObject
    public Text JumpKeyText;
    public Text LeftKeyText;
    public Text RightKeyText;
    public Text InteractKeyText;
    void Start()
    {
        // 寫好按鈕上預設的字
        JumpKeyText.text = GlobalVariables.Instance.JumpKey.ToString();
        LeftKeyText.text = GlobalVariables.Instance.MoveLeftKey.ToString();
        RightKeyText.text = GlobalVariables.Instance.MoveRightKey.ToString();
        InteractKeyText.text = GlobalVariables.Instance.InteractKey.ToString();
    }

// ---- 下面是給不同按鈕的callback function -------
    public void BindInteractKey()
    {
        // 開啟一個Coroutine去偵測有哪個按鈕被press， \
        // 並且用那個按鈕去call傳進去的callback function
        StartCoroutine(BindKey((k)=>{
            GlobalVariables.Instance.InteractKey = k;
            InteractKeyText.text = k.ToString();
            BindingBlur.SetActive(false);
        }));
    }
    public void BindRightKey()
    {
        StartCoroutine(BindKey((k)=>{
            GlobalVariables.Instance.MoveRightKey = k;
            RightKeyText.text = k.ToString();
            BindingBlur.SetActive(false);
        }));
    }
    public void BindLeftKey()
    {
        StartCoroutine(BindKey((k)=>{
            GlobalVariables.Instance.MoveLeftKey = k;
            LeftKeyText.text = k.ToString();
            BindingBlur.SetActive(false);
        }));
    }
    public void BindJumpKey()
    {
        StartCoroutine(BindKey((k)=>{
            GlobalVariables.Instance.JumpKey = k;
            JumpKeyText.text = k.ToString();
            BindingBlur.SetActive(false);
        }));
    }
    public IEnumerator BindKey(Action<Key> callback)
    {
        BindingBlur.SetActive(true);
        IsBinding = true;
        bool done = false;
        while (!done) // 重複直到有按鈕被按下
                      // 因為不一定會馬上被按下，所以這個function是IEnumerator function
        {
            if (!Keyboard.current.anyKey.wasPressedThisFrame)
            {
                yield return null;
                continue;
            }
            
            if (Keyboard.current.escapeKey.wasPressedThisFrame) {
                BindingBlur.SetActive(false);
                break;
            }

            foreach(KeyControl kc in Keyboard.current.allKeys)
            {
                if (kc.wasPressedThisFrame)
                {
                    callback(kc.keyCode);
                    done = true;
                    break;
                }
            }
            yield return null;
        }
        IsBinding = false;
    }
}
