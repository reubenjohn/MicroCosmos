using UnityEngine;

public static class Serialization
{
    public static string ToSerializable(Vector2 vec) => $"{vec.x} {vec.y}";

    public static Vector2 ToVector2(string vs)
    {
        var xy = vs.Split(' ');
        return new Vector2(float.Parse(xy[0]), float.Parse(xy[1]));
    }
}