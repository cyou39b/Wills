using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    void Awake() { audioSource = GetComponent<AudioSource>();}

    bool initialized = false;
    public void Initialize(AudioClip clip, float minPitch = 1.0f, float maxPitch = 1.0f)
    {
        initialized = true;
        if(audioSource == null) {return;}
        audioSource.clip = clip;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }

    void Update()
    {
        if(initialized && !audioSource.isPlaying){Destroy(gameObject);}
    }
}
