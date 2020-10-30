using UnityEngine;

public static class Serialization
{
    public static float[] ToSerializable(Vector2 vec) => new float[] { vec.x, vec.y };
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