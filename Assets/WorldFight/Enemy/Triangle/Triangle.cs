using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Triangle : MonoBehaviour
{
    private Camera cam;
    private SpriteRenderer spRerr = null;
    [NonSerialized] public AbstractEnemy wills1;

    private Color _wills1Color;
    public Color wills1Color
    {
        get => _wills1Color;
        set
        {
            _wills1Color = value;
            if(spRerr != null)
            {
                spRerr.color = value;
            }
        }
    }
    public void Initialize(AbstractEnemy targetWills1, Color color)
    {
        wills1 = targetWills1;
        wills1Color = color;
    }

    void Start()
    {
        cam = Camera.main;

        spRerr = GetComponent<SpriteRenderer>();
        spRerr.color = _wills1Color;
    }

    void Update()
    {
        Vector3 onScreenPos = cam.WorldToViewportPoint(wills1.transform.position);
        onScreenPos -= new Vector3(0.5f, 0.5f, onScreenPos.z);
        float viewPortDistance = onScreenPos.magnitude;
        
        Color sprColor = wills1.mainColor;
        if(viewPortDistance <= 0.6f)
        {
            sprColor.a = 0.0f;
        }
        else if(viewPortDistance <= 0.8f)
        {
            sprColor.a = (viewPortDistance - 0.6f) / 0.2f;
        }
        else
        {
            sprColor.a = 1.0f;
        }
        wills1Color = sprColor;
        spRerr.color = sprColor;

        Vector3 diff = wills1.transform.position - cam.transform.position;
        diff.z = 0.0f;
        transform.rotation = Quaternion.Euler(
            0.0f,
            0.0f,
            Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg
        );
        diff *= 0.47f / Mathf.Max(Mathf.Abs(diff.x), Mathf.Abs(diff.y));
        diff += new Vector3(0.5f, 0.5f);

        Vector3 worldPos = cam.ViewportToWorldPoint(diff);
        worldPos.z = transform.position.z;
        transform.position = worldPos;
    }
}
