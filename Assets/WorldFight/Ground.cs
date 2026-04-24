using UnityEngine;

public class Ground : MonoBehaviour
{
    public ParticleSystem.MinMaxCurve VelocityToVolume;
    public AudioClip fallSFX;
    public GameObject soundEffectPlayerPrefab;
    void OnCollisionEnter2D(Collision2D collision)
    {
        float volume = VelocityToVolume.Evaluate(-collision.relativeVelocity.y);

        // Debug.Log($"{collision.relativeVelocity.y}, {volume}, {collision.collider.name}");
        if(volume > 0.0f)
        {
            GameObject newObj = Instantiate(soundEffectPlayerPrefab, transform.position, transform.rotation);
            SoundEffectPlayer soundEffectPlayer;
            if(!newObj.TryGetComponent<SoundEffectPlayer>(out soundEffectPlayer))
            {
                Debug.LogError("Missing component");
            }
            soundEffectPlayer.Initialize(fallSFX, 0.9f, 1.1f, volume, 1.0f);
        }
    }
}
