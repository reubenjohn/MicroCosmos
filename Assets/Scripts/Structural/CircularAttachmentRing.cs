using UnityEngine;

namespace Structural
{
    public class CircularAttachmentRing
    {
        public void AttachAt(CircularAttachment attachment)
        {
            // TODO Handle displacement contention
            attachment.Transform.localRotation = Quaternion.Euler(0, 0, attachment.PreferredAngle);
            attachment.Transform.localPosition = attachment.Transform.localRotation * (Vector3.up * .5f);
        }
    }
}