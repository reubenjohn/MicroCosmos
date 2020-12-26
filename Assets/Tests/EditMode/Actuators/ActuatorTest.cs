using NUnit.Framework;
using Organelles.Flagella;
using UnityEngine;

namespace Tests.EditMode.Actuators
{
    public class ActuatorTest
    {
        [Test]
        public void TestFlagellaRelativeForce()
        {
            var gene = new FlagellaGene(100f, 2f);
            var relativeForce = FlagellaActuator.CalculateRelativeForce(gene, new[] {.2f, .3f}, .25f, .1f);

            Assert.AreEqual(new Vector2(0, .5f), relativeForce);
        }

        [Test]
        public void TestFlagellaTorque()
        {
            var gene = new FlagellaGene(100f, 2f);

            var torque = FlagellaActuator.CalculateTorque(gene, new[] {.2f, .3f}, .5f, .1f);
            Assert.IsTrue(Mathf.Approximately(.03f, torque));

            var torque2 = FlagellaActuator.CalculateTorque(gene, new[] {.2f, -1f}, 1f, .1f);
            Assert.IsTrue(Mathf.Approximately(-.2f, torque2));
        }

        [Test]
        public void TestFlagellaGeneTranscriberSampling()
        {
            Random.InitState(0);
            var gene = FlagellaGeneTranscriber.Singleton.Sample();
            Assert.AreEqual(67.8766785f, gene.linearPower);
            Assert.AreEqual(678.946045f, gene.angularPower);
        }
    }
}