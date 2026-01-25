using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 附加在menu中每一個選項上的Script，讓滑鼠移動到選向上時給玩家一點feedback
// 我覺得這個class的code算是self-explaining了

public class MouseInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioSource WhooshSFX;
    public Text txt;
    private static readonly Color hoveredColor = new Color(200.0f/255.5f, 200.0f/255.5f, 200.0f/255.5f);
    private static readonly Color normalColor = new Color(26/255.5f, 26/255.5f, 26/255.5f);
    public void OnPointerEnter(PointerEventData ed)
    {
        WhooshSFX.Play();
        txt.color = hoveredColor;
    }
    public void OnPointerExit(PointerEventData ed)
    {
        txt.color = normalColor;
    }
}
