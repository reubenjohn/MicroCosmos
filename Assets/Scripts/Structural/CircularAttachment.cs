using UnityEngine;

namespace DefaultNamespace
{
    public class CircularAttachment
    {
        public Transform Transform { get; }
        public float PreferredAngle { get; }
        public float AngularDisplacement { get; }

        public CircularAttachment(Transform transform, float preferredAngle, float angularDisplacement)
        {
            PreferredAngle = preferredAngle;
            Transform = transform;
            AngularDisplacement = angularDisplacement;
        }
    }
}