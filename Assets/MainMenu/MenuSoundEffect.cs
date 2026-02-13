using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MenuSoundEffect : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] clips;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private float coolDownTimer = 0.0f;
    void Update()
    {
        coolDownTimer -= Time.unscaledDeltaTime;
    }
    public void PlaySoundEffectIfPossible()
    {
        if (!audioSource.isPlaying)
        {
            if(coolDownTimer <= 0.0f)
            {
                AudioClip clipToPlay = clips[Random.Range(1, clips.Length)];
                audioSource.clip = clipToPlay;
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.Play();
                coolDownTimer = Random.Range(10.0f, 15.0f);
            }
        }
    }
}
