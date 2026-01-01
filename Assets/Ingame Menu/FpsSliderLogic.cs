using UnityEngine;
using UnityEngine.UI;

public class FpsSliderLogic : MonoBehaviour
{
    public Text FpsText;
    public Slider FpsSlider;
    public void Start()
    {
        FpsSlider.value = GlobalVariables.Instance.FrameRate;
    }
    public void OnValueChange()
    {
        FpsText.text = FpsSlider.value.ToString();
        GlobalVariables.Instance.FrameRate = (int)FpsSlider.value;
        Application.targetFrameRate = GlobalVariables.Instance.FrameRate;
    }
}
