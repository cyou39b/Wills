using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MouseInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioSource WhooshSFX;
    public Text txt;
    // WHY THE FXCK UNITY DID CHOOSE TO USE 255 AS MAX IN INSPECTOR BUT 1.0f AS MAX IN CODE????????
    private static readonly Color hoverColor = new Color(200.0f/255.5f, 200.0f/255.5f, 200.0f/255.5f);
    private static readonly Color normalColor = new Color(26/255.5f, 26/255.5f, 26/255.5f);
    public void OnPointerEnter(PointerEventData ed)
    {
        WhooshSFX.Play();
        txt.color = hoverColor;
    }
    public void OnPointerExit(PointerEventData ed)
    {
        txt.color = normalColor;
    }
}
