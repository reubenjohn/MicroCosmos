using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class GeneTranscriberTest
    {
        [Test]
        public void SerializationDeserialization()
        {
            var gene = new SampleGene(.5f, 2, DietaryRestriction.HERBIVORE, new Limb[] { new Limb(3.4f), new Limb(3.4f) });
            var sequence = JsonConvert.SerializeObject(gene);
            Assert.AreEqual("{\"furryness\":0.5,\"nEyes\":2,\"dietaryRestriction\":0,\"limbs\":[{\"length\":3.4},{\"length\":3.4}]}", sequence);
            var deserializedGene = SampleGeneTranscriber.SINGLETON.Deserialize(JObject.Parse(sequence));
            Assert.AreNotSame(gene, deserializedGene);
            Assert.AreEqual(gene, deserializedGene);
        }

        [Test]
        public void Mutate()
        {
            var gene = new SampleGene(.5f, 2, DietaryRestriction.HERBIVORE, new Limb[] { new Limb(3.4f), new Limb(3.4f) });
            Random.InitState(1);
            var mutated = SampleGeneTranscriber.SINGLETON.Mutate(gene);
            var expectedMutation = new SampleGene(0.400063068f, 2, DietaryRestriction.HERBIVORE, new Limb[] { new Limb(3.407909f), new Limb(3.38111448f) });
            Assert.AreEqual(expectedMutation, mutated);
        }
    }
}
