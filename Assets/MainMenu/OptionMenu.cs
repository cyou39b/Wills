using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    public GameObject BindingBlur;
    public Text JumpKeyText;
    void Start()
    {
        JumpKeyText.text = GlobalVariables.Instance.JumpKey.ToString();
    }
    public void BindJumpKey()
    {
        BindingBlur.SetActive(true);
        StartCoroutine(BindKey((k)=>{
            GlobalVariables.Instance.JumpKey = k;
            JumpKeyText.text = k.ToString();
            BindingBlur.SetActive(false);
        }));
    }
    public IEnumerator BindKey(Action<Key> callback)
    {
        while (true)
        {
            if (!Keyboard.current.anyKey.wasPressedThisFrame)
            {
                yield return null;
                continue;
            }
            
            if (Keyboard.current.escapeKey.wasPressedThisFrame) {break;}

            foreach(var kc in Keyboard.current.allKeys)
            {
                if (kc.wasPressedThisFrame)
                {
                    callback(kc.keyCode);
                    break;
                }
            }
            yield return null;
        }
    }
}
