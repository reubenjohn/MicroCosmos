using System.Collections.Generic;
using UnityEngine;

public static class TransformUtils
{
    public static IEnumerable<Transform> Children(this Transform parent)
    {
        foreach (Transform t in parent)
            yield return t;
    }
}