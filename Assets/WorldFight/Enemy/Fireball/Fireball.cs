using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private static readonly WaitForSeconds waitForOneSecond = new WaitForSeconds(1.0f);
    IEnumerator Start()
    {
        yield return waitForOneSecond; // TODO:
        Destroy(gameObject);
    }
}
