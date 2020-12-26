using UnityEngine;

namespace Structural
{
    public class CircularAttachment
    {
        public CircularAttachment(Transform transform, CircularAttachmentGene attachmentGene)
        {
            Transform = transform;
            PreferredAngle = attachmentGene.preferredAngle;
            AngularDisplacement = attachmentGene.angularDisplacement;
        }

        public Transform Transform { get; }
        public float PreferredAngle { get; }
        public float AngularDisplacement { get; }
    }
}