using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeSliderLogic : MonoBehaviour, IPointerUpHandler
{
    public AudioSource ReleaseSFX;
    public Slider VolumeSlider;
    public Text VolumeText;

    public void OnValueChange()
    {
        VolumeText.text = Math.Round(VolumeSlider.value*100f).ToString()+"%";
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        AudioListener.volume = VolumeSlider.value;
        ReleaseSFX.Play();
    }
}
