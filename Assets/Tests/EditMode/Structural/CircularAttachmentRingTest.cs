﻿using DefaultNamespace;
using NUnit.Framework;
using Structural;
using UnityEngine;

namespace Tests.EditMode.Structural
{
    public class CircularAttachmentRingTest
    {
        [Test]
        public void AttachmentsArePlacedCorrectly()
        {
            var membraneObj = (GameObject) Object.Instantiate(Resources.Load("Organelles/Membrane1"));
            var circularAttachmentRing = new CircularAttachmentRing();

            circularAttachmentRing.AttachAt(new CircularAttachment(membraneObj.transform, 0f, 15f));
            Assert.AreEqual(new Vector3(0, .5f, 0), membraneObj.transform.position);

            circularAttachmentRing.AttachAt(new CircularAttachment(membraneObj.transform, 90f, 15f));
            // ReSharper disable once Unity.InefficientPropertyAccess
            Assert.AreEqual(new Vector3(-.5f, 0, 0).ToString(), membraneObj.transform.position.ToString());
        }
    }
}