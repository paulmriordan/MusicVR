using UnityEngine;

static class UnityExtensions
{
    public static Vector2 ToVector2XZ(this Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.z);
    }

    public static Vector3 ToVector3XZ(this Vector2 vec2, float y = 0)
    {
        return new Vector3(vec2.x, y, vec2.y);
    }
}
