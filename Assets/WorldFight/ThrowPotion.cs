using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowPotion : MonoBehaviour
{
    public static bool inThrow = false;
    GameObject potionGameobject;
    Camera cam;
    void Start()
    {
        cam = Camera.main;
        Transform potionTransform = transform.Find("potion");
        if(potionTransform == null)
        {
            Debug.LogError("Cant find child object potion");
        }
        potionGameobject = potionTransform.gameObject;
    }

    void Update()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        float rot = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
        Debug.Log($"{mousePos - transform.position}, {rot}");

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Rad2Deg * rot + 90.0f);
        potionGameobject.transform.rotation = Quaternion.identity;
    }

    public float ThrowVelocity;
    public float effectAmount;
    public Action throwCallback;
    public GameObject PotionPrefab;
    public void TryThrow()
    {
        if(!enabled || !gameObject.activeSelf) {return;}

        GameObject newObj = Instantiate(PotionPrefab, transform.position, Quaternion.identity);
        HealingPotion healingPotion;
        if(!newObj.TryGetComponent<HealingPotion>(out healingPotion))
        {
            Debug.LogError("Missing component");
        }

        Vector3 dir = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
        Vector2 velocity = new Vector2(dir.x, dir.y).normalized * ThrowVelocity;
        

        healingPotion.Initialize(effectAmount, velocity);
        throwCallback();
    }
}
