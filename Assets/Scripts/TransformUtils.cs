using UnityEngine;
using System.Collections.Generic;

public static class TransformUtils
{
    public static IEnumerable<Transform> Children(this Transform parent)
    {
        foreach (Transform t in parent)
            yield return t;
    }

}
