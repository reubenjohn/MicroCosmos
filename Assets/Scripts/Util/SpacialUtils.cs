using UnityEngine;

namespace Util
{
    public static class SpacialUtils
    {
        private static readonly float PI_2 = 2 * Mathf.PI;

        public static float NormalizeOrientation(float orientation)
        {
            orientation %= PI_2;
            if (Mathf.Abs(orientation) > PI_2)
                orientation %= PI_2;
            if (orientation > Mathf.PI)
                orientation -= PI_2;
            if (orientation < -Mathf.PI)
                orientation += PI_2;
            return orientation;
        }
    }
}