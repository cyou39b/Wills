using UnityEngine;
using System;

// I was thinking maybe we should have something like this, but it didn't work
// out so well, I didn't delete this thing bc I might find this thing usful later.
public class CursorManager: MonoBehaviour
{
    public CrusorData crusorData;
    public static string CurrentCursorName = "Normal";
    private string displayingCursorName = "";

    void Update()
    {
        if(!displayingCursorName.Equals(CurrentCursorName))
        {
            displayingCursorName = CurrentCursorName;
            int idx = Array.FindIndex(crusorData.Names, name => name.Equals(displayingCursorName));
            if(idx == -1)
            {
                Debug.LogError($"Can't find Cursor Texture named \"{CurrentCursorName}\"");
            }

            Cursor.SetCursor(crusorData.Textures[idx], Vector2.zero, CursorMode.Auto);
        }
    }
}
