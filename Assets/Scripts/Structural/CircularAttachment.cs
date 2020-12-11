using UnityEngine;

namespace Structural
{
    public class CircularAttachment
    {
        public CircularAttachment(Transform transform, float preferredAngle, float angularDisplacement)
        {
            PreferredAngle = preferredAngle;
            Transform = transform;
            AngularDisplacement = angularDisplacement;
        }

        public Transform Transform { get; }
        public float PreferredAngle { get; }
        public float AngularDisplacement { get; }
    }
}