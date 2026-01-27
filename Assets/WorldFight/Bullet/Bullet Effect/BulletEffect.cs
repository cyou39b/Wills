using System.Collections;
using UnityEngine;

public class BulletEffect : MonoBehaviour
{
    private Animator anmor;
    public RuntimeAnimatorController[] Animations;
    public void Start()
    {
        anmor = GetComponent<Animator>();
        anmor.runtimeAnimatorController = 
            Animations[Random.Range(0, Animations.Length)];
            StartCoroutine(DestroyAfterAnimation());
    }

    private static readonly WaitForSeconds wForAnimation = new WaitForSeconds(6.5f/24.0f);
    public IEnumerator DestroyAfterAnimation()
    {
        yield return wForAnimation;
        Destroy(gameObject);
    }
}
