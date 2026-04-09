using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    void Start() { audioSource = GetComponent<AudioSource>();}

    public void Initialize(AudioClip clip, float minPitch = 1.0f, float maxPitch = 1.0f)
    {
        if(audioSource == null) {return;}
        audioSource.clip = clip;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }

    void Update()
    {
        if(!audioSource.isPlaying){Destroy(gameObject);}
    }
}
