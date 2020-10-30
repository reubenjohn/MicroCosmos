using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class GeneTest
    {
        [Test]
        public void SerializationDeserialization()
        {
            var gene = new TestGene(.5f, 2, TestGene.DietaryRestriction.HERBIVORE, new TestGene.Limb[] { new TestGene.Limb(3.4f), new TestGene.Limb(3.4f) });
            var sequence = gene.Serialize();
            Assert.AreEqual("{\"furryness\":0.5,\"nEyes\":2,\"dietaryRestriction\":0,\"limbs\":[{\"length\":3.4},{\"length\":3.4}]}", sequence);
            var deserializedGene = gene.Deserialize(sequence);
            Assert.AreNotSame(gene, deserializedGene);
            Assert.AreEqual(gene, deserializedGene);
        }

        // [Test]
        // public void Duplicate()
        // {
        //     var gene = new TestGene(.5f, 2, TestGene.DietaryRestriction.HERBIVORE, new TestGene.Limb[] { new TestGene.Limb(3.4f), new TestGene.Limb(3.4f) });
        //     var duplicate = gene.Duplicate();
        //     Assert.AreNotSame(gene, duplicate);
        //     Assert.AreEqual(gene, duplicate);
        // }

        [Test]
        public void Mutate()
        {
            var gene = new TestGene(.5f, 2, TestGene.DietaryRestriction.HERBIVORE, new TestGene.Limb[] { new TestGene.Limb(3.4f), new TestGene.Limb(3.4f) });
            Random.InitState(1);
            var mutated = gene.Mutate();
            var expectedMutation = new TestGene(0.400063068f, 2, TestGene.DietaryRestriction.HERBIVORE, new TestGene.Limb[] { new TestGene.Limb(3.407909f), new TestGene.Limb(3.38111448f) });
            Assert.AreEqual(expectedMutation, mutated);
        }
    }
}
