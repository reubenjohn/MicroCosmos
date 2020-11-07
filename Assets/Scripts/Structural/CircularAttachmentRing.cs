using DefaultNamespace;
using UnityEngine;

namespace Structural
{
    public class CircularAttachmentRing
    {
        private float Radius { get; }

        public CircularAttachmentRing(float radius)
        {
            Radius = radius;
        }

        public void AttachAt(CircularAttachment attachment)
        {
            // TODO Handle displacement contention
            attachment.Transform.localRotation = Quaternion.Euler(0, 0, attachment.PreferredAngle);
            attachment.Transform.localPosition = attachment.Transform.localRotation * (Vector3.up * .5f);
        }
    }
}