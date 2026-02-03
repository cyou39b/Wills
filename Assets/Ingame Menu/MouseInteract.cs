using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 附加在menu中每一個選項上的Script，讓滑鼠移動到選向上時給玩家一點feedback
// 我覺得這個class的code算是self-explaining了

public class MouseInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioSource WhooshSFX;
    public Text txt;
    private static readonly Color hoveredColor = new Color(0.0207f, 0.6969f, 1.0f);
    private static readonly Color normalColor = new Color(0.6622f, 0.8959f, 1.0f);

    public void Start()
    {
        txt.color = normalColor;
    }
 
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
