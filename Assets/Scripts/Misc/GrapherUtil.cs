using System;
using Chemistry;
using UnityEngine;

public static class GrapherUtil
{
    public static void LogFlask<T>(Flask<T> flask, string flaskName, int interval, bool enabled = true) where T : Enum
    {
        if (!enabled || Time.frameCount % interval != 0) return;

        var enumType = typeof(T);
        Grapher.Log(flask.TotalMass, $"{flaskName}.TotalMass");
        for (var i = 0; i < flask.Length; i++)
        {
            var substance = (T) Enum.ToObject(enumType, i);
            Grapher.Log(flask[substance], $"{flaskName}.{substance}");
        }
    }
}