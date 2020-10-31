using Newtonsoft.Json.Linq;
using UnityEngine;

public static class Serialization
{
    public static JArray ToSerializable(Vector2 vec) => new JArray(vec.x, vec.y);
    public static float[] ToSerializable(Quaternion rot) => new float[] { rot.x, rot.y, rot.z, rot.w };

    public static Quaternion ToQuaternion(float[] vs)
    {
        return new Quaternion(vs[0], vs[1], vs[2], vs[3]);
    }

    public static Vector2 ToVector2(float[] vs)
    {
        return new Vector2(vs[0], vs[1]);
    }
}