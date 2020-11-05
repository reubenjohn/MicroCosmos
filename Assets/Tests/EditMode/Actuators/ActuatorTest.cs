using Actuators;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode.Actuators
{
    public class ActuatorTest
    {
        [Test]
        public void TestFlagellaRelativeForce()
        {
            var gene = new FlagellaGene(100f, 2f);
            var relativeForce = FlagellaActuator.CalculateRelativeForce(gene, new[] {.2f, .3f}, .1f);

            Assert.AreEqual(new Vector2(0, 2f), relativeForce);
        }

        [Test]
        public void TestFlagellaTorque()
        {
            var gene = new FlagellaGene(100f, 2f);
            var torque = FlagellaActuator.CalculateTorque(gene, new[] {.2f, .3f}, .1f);

            Assert.IsTrue(Mathf.Approximately(.06f, torque));
        }
    }
}