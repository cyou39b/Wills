using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIStack : MonoBehaviour
{
    public static UIStack Instance{get; private set;} = null;

    [System.NonSerialized] public System.Action emptyAction = () => {};
    private List<System.Action> stack = new List<System.Action>();

    /// <summary>
    /// Register a new panel to the UIStack.
    /// </summary>
    /// <returns>id of new panel</returns>
    public int NewPanel(System.Action closeAction)
    {
        stack.Add(closeAction);
        return stack.Count - 1;
    }

    public void RemovePanel(int idx)
    {
        for(int i=stack.Count-1;i>=idx;i--)
        {
            stack[i].Invoke();
        }

        stack.RemoveRange(idx, stack.Count - idx);
    }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if(stack.Count > 0)
            {
                RemovePanel(stack.Count-1);
            }
            else
            {
                emptyAction();
            }
        }
    }

    void OnDestroy()
    {
        RemovePanel(0);
    }
}
