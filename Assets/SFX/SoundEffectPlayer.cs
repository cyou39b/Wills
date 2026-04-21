using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private bool oneShot = false;
    private float endTime = float.PositiveInfinity;
    void Awake() { audioSource = GetComponent<AudioSource>();}

    bool initialized = false;
    public void Initialize(AudioClip clip, float minPitch = 1.0f, float maxPitch = 1.0f, float volume = 1.0f, float blend = 0.75f, bool oneShot = false)
    {
        initialized = true;
        this.oneShot = oneShot;
        if(audioSource == null) {return;}
        audioSource.clip = clip;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.spatialBlend = blend;
        if(oneShot)
        {
            audioSource.PlayOneShot(clip, volume);
            endTime = Time.realtimeSinceStartup + clip.length;
        }
        else
        {
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

    void Update()
    {
        if(initialized && (oneShot?Time.realtimeSinceStartup>=endTime:!audioSource.isPlaying)){Destroy(gameObject);}
    }
}
