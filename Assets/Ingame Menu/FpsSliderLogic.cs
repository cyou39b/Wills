using UnityEngine;
using UnityEngine.UI;

// 附加在調整FPS的Slider上的Script
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
        // 在Slider被拉動時更新遊戲的FPS以及顯示FPS的字
        FpsText.text = FpsSlider.value.ToString();
        GlobalVariables.Instance.FrameRate = (int)FpsSlider.value;
        Application.targetFrameRate = GlobalVariables.Instance.FrameRate;
    }
}
