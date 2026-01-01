using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    public static bool IsBinding = false;
    public GameObject BindingBlur;
    public Text JumpKeyText;
    public Text LeftKeyText;
    public Text RightKeyText;
    public Text InteractKeyText;
    void Start()
    {
        JumpKeyText.text = GlobalVariables.Instance.JumpKey.ToString();
        LeftKeyText.text = GlobalVariables.Instance.MoveLeftKey.ToString();
        RightKeyText.text = GlobalVariables.Instance.MoveRightKey.ToString();
        InteractKeyText.text = GlobalVariables.Instance.InteractKey.ToString();
    }

    public void BindInteractKey()
    {
        BindingBlur.SetActive(true);
        IsBinding = true;
        StartCoroutine(BindKey((k)=>{
            GlobalVariables.Instance.InteractKey = k;
            InteractKeyText.text = k.ToString();
            BindingBlur.SetActive(false);
        }));
    }
    public void BindRightKey()
    {
        BindingBlur.SetActive(true);
        IsBinding = true;
        StartCoroutine(BindKey((k)=>{
            GlobalVariables.Instance.MoveRightKey = k;
            RightKeyText.text = k.ToString();
            BindingBlur.SetActive(false);
        }));
    }
    public void BindLeftKey()
    {
        BindingBlur.SetActive(true);
        IsBinding = true;
        StartCoroutine(BindKey((k)=>{
            GlobalVariables.Instance.MoveLeftKey = k;
            LeftKeyText.text = k.ToString();
            BindingBlur.SetActive(false);
        }));
    }
    public void BindJumpKey()
    {
        BindingBlur.SetActive(true);
        IsBinding = true;
        StartCoroutine(BindKey((k)=>{
            GlobalVariables.Instance.JumpKey = k;
            JumpKeyText.text = k.ToString();
            BindingBlur.SetActive(false);
        }));
    }
    public IEnumerator BindKey(Action<Key> callback)
    {
        bool done = false;
        while (!done)
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
