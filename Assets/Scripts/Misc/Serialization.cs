using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public static class Serialization
{
    public static string ToSerializable(Vector2 vec) => $"{vec.x} {vec.y}";

    public static Vector2 ToVector2(string vs)
    {
        var xy = vs.Split(' ');
        return new Vector2(float.Parse(xy[0]), float.Parse(xy[1]));
    }

    public static string ToPrintable(this float[] arr) => JsonConvert.SerializeObject(arr);

    public static string ToPrintable(this float[] arr, int precision) =>
        JsonConvert.SerializeObject(arr.Select(f => Math.Round(f, precision)));

    public static string ToPrintable(this float[][] arr, int precision) =>
        $"[{string.Join(",", arr.Select(a => a.ToPrintable(precision)))}]";
}