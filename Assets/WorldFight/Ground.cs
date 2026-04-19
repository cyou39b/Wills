using UnityEngine;

public class Ground : MonoBehaviour
{
    public ParticleSystem.MinMaxCurve VelocityToVolume;
    public AudioClip fallSFX;
    public GameObject soundEffectPlayerPrefab;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(VelocityToVolume.Evaluate(Mathf.Abs(collision.relativeVelocity.y)) > 0.0f)
        {
            GameObject newObj = Instantiate(soundEffectPlayerPrefab, transform.position, transform.rotation);
            SoundEffectPlayer soundEffectPlayer;
            if(!newObj.TryGetComponent<SoundEffectPlayer>(out soundEffectPlayer))
            {
                Debug.LogError("Missing component");
            }
            soundEffectPlayer.Initialize(fallSFX, 0.9f, 1.1f);
        }
    }
}
