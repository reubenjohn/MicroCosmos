using System.Linq;
using Brains;
using Brains.SimpleGeneticBrain1;
using Genetics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;
using Util;

namespace Tests.EditMode.Brains.SimpleGeneticBrain1
{
    public static class SimpleGeneticBrain1GeneTranscriberTest
    {
        private static readonly string Json1 =
            @"{""denseLayer1"":{""Biases"":[-0.361967683],""Weights"":[[-0.9993694,-0.5485256]]}}";

        private static readonly RepairableGeneTranscriber<SimpleGeneticBrain1Gene, SimpleGeneticBrain1Description>
            Transcriber = SimpleGeneticBrain1GeneTranscriber.Singleton;

        [Test]
        public static void SerializeTest()
        {
            Random.InitState(1);
            var gene = new SimpleGeneticBrain1Gene(SimpleGeneticBrain1GeneTranscriber.Repairer)
            {
                denseLayer1 = new DenseLayerGene(RandomUtils.RandomLogits(1, 2), RandomUtils.RandomLogits(1))
            };
            Assert.AreEqual(Json1, JsonConvert.SerializeObject(gene));
        }

        [Test]
        public static void DeserializeTest()
        {
            Random.InitState(1);
            var gene = Transcriber.Deserialize(JToken.Parse(Json1));
            Assert.AreEqual(Json1, JsonConvert.SerializeObject(gene));
        }

        [Test]
        public static void MutateTest()
        {
            Random.InitState(1);
            var gene = Transcriber.Deserialize(JToken.Parse(Json1));
            var mutated = Transcriber.Mutate(gene);
            Assert.AreEqual(
                @"{""denseLayer1"":{""Biases"":[-0.3800661],""Weights"":[[-1.0,-0.5759519]]}}",
                JsonConvert.SerializeObject(mutated));
        }

        [Test]
        public static void NoRepairNeededTest()
        {
            Random.InitState(1);
            IRepairableGene<SimpleGeneticBrain1Gene, SimpleGeneticBrain1Description> gene = Transcriber.Sample();
            var repairedGene = gene.RepairGene(new SimpleGeneticBrain1Description(1, 1));
            Assert.AreSame(((SimpleGeneticBrain1Gene) gene).denseLayer1, repairedGene.denseLayer1);
        }

        [Test]
        public static void SamplingTest()
        {
            Random.InitState(1);
            var gene = Transcriber.Sample();
            Assert.AreSame(Transcriber.GetRepairer(), gene.repairer);
            var dense1 = gene.denseLayer1;
            Assert.AreEqual(2, dense1.Weights.GetLength(0));
            Assert.AreEqual(3, dense1.Weights.GetLength(1));
            Assert.AreEqual(2, dense1.Biases.Length);
            Assert.AreEqual(-0.103475809f, dense1.Biases.Sum());
            var enumerator = dense1.Weights.GetEnumerator();
            var count = 0f;
            var sum = 0.0f;
            while (enumerator.MoveNext())
            {
                count++;
                // ReSharper disable once PossibleNullReferenceException
                sum += (float) enumerator.Current;
            }

            Assert.AreEqual(-2.58920884f, sum);
            Assert.AreEqual(2 * 3, count);
        }

        [Test]
        public static void RepairNeededTest()
        {
            Random.InitState(1);
            IRepairableGene<SimpleGeneticBrain1Gene, SimpleGeneticBrain1Description> gene = Transcriber.Sample();
            var simpleGeneticBrain1Gene = (SimpleGeneticBrain1Gene) gene;
            var geneStr = JsonConvert.SerializeObject(simpleGeneticBrain1Gene.denseLayer1);
            var repairedGene = gene.RepairGene(new SimpleGeneticBrain1Description(1, 3));
            Assert.AreSame(SimpleGeneticBrain1GeneTranscriber.Repairer, repairedGene.repairer);
            Debug.Log(JsonConvert.SerializeObject(repairedGene.denseLayer1));
            Assert.AreEqual(
                @"{""Biases"":[-0.828767538,0.7252917,0.486216515],""Weights"":[[-0.9993694,-0.5485256,-0.361967683],[0.079087615,-0.188854814,-0.5695789],[0.0,0.0,0.0]]}",
                JsonConvert.SerializeObject(repairedGene.denseLayer1));

            simpleGeneticBrain1Gene.denseLayer1 = repairedGene.denseLayer1 = null;
            Assert.AreEqual(JsonConvert.SerializeObject(simpleGeneticBrain1Gene),
                JsonConvert.SerializeObject(repairedGene));
        }
    }
}