using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Tests.Common;
using UnityEngine;

namespace Tests.EditMode.Genetics
{
    public class GeneTranscriberTest
    {
        [Test]
        public void SerializationDeserialization()
        {
            var gene = new SampleGene(.5f, 2, DietaryRestriction.Herbivore, new[] { new Limb(3.4f), new Limb(3.4f) });
            var sequence = JsonConvert.SerializeObject(gene);
            Assert.AreEqual(SampleGene.Serialized1, sequence);
            var deserializedGene = SampleGeneTranscriber.Singleton.Deserialize(JObject.Parse(sequence));
            Assert.AreNotSame(gene, deserializedGene);
            Assert.AreEqual(gene, deserializedGene);
        }

        [Test]
        public void Mutate()
        {
            var gene = new SampleGene(.5f, 2, DietaryRestriction.Herbivore, new[] { new Limb(3.4f), new Limb(3.4f) });
            Random.InitState(1);
            var mutated = SampleGeneTranscriber.Singleton.Mutate(gene);
            var expectedMutation = new SampleGene(0.400063068f, 2, DietaryRestriction.Herbivore, new[] { new Limb(3.407909f), new Limb(3.38111448f) });
            Assert.AreEqual(expectedMutation, mutated);
        }
    }
}
