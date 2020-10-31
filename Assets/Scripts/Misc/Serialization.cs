using Newtonsoft.Json.Linq;
using UnityEngine;

public static class Serialization
{
    public static string ToSerializable(Vector2 vec) => string.Format("{0} {1}", vec.x, vec.y);
    public static Vector2 ToVector2(string vs)
    {
        string[] xy = vs.Split(' ');
        return new Vector2(float.Parse(xy[0]), float.Parse(xy[1]));
    }
}