using UnityEngine;

public class WaitForFrames : CustomYieldInstruction
{
    public int _TargetFrame;

    public WaitForFrames(int frames)
    {
        _TargetFrame = Time.frameCount + frames;
    }

    public override bool keepWaiting
    {
        get => Time.frameCount < _TargetFrame;
    }
}