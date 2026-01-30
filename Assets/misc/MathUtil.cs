using UnityEngine;

public static class MathUtil
{
    // Some useful constants
    public const float PI = Mathf.PI;
    public const float NPI = - Mathf.PI;
    public const float HalfPI = PI / 2;
    public const float HalfNPI = NPI / 2;
    
    public static Vector3 Vector2ToVecotr3(Vector2 a, float z = 0.0f)
    {
        return new Vector3(a.x, a.y, z);
    }

    public static Vector2 RandomPointInCircle(Vector2 center, float radius)
    {
        float r = Mathf.Sqrt(Random.Range(0.0f, 1.0f)) * radius;
        float rot = Random.Range(NPI, PI);
        center.x += r * Mathf.Cos(rot);
        center.y += r * Mathf.Sin(rot);
        return center;
    }

    public static Vector2 RotateVector2(Vector2 a, float rot)
    {
        float dir = Mathf.Atan2(a.y, a.x);
        float r = a.magnitude;
        a.x = r * Mathf.Cos(dir);
        a.y = r * Mathf.Sin(dir);
        return a;
    }

    public static Vector3 AddVectors(Vector3 v3, Vector2 v2)
    {
        v3.x += v2.x;
        v3.y += v2.y;
        return v3;
    }
}