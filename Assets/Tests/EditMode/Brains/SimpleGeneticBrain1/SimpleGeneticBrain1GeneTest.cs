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
            Assert.AreEqual(8, dense1.Weights.GetLength(0));
            Assert.AreEqual(16, dense1.Weights.GetLength(1));
            Assert.AreEqual(8, dense1.Biases.Length);
            Assert.AreEqual(0.920621872f, dense1.Biases.Sum());
            var enumerator = dense1.Weights.GetEnumerator();
            var count = 0f;
            var sum = 0.0f;
            while (enumerator.MoveNext())
            {
                count++;
                // ReSharper disable once PossibleNullReferenceException
                sum += (float) enumerator.Current;
            }

            Assert.AreEqual(-1.50783443f, sum);
            Assert.AreEqual(8 * 16, count);
        }

        [Test]
        public static void RepairNeededTest()
        {
            Random.InitState(1);
            IRepairableGene<SimpleGeneticBrain1Gene, SimpleGeneticBrain1Description> gene = Transcriber.Sample();
            var simpleGeneticBrain1Gene = (SimpleGeneticBrain1Gene) gene;
            var geneStr = JsonConvert.SerializeObject(simpleGeneticBrain1Gene.denseLayer1);
            var repairedGene = gene.RepairGene(new SimpleGeneticBrain1Description(18, 9));
            Assert.AreSame(SimpleGeneticBrain1GeneTranscriber.Repairer, repairedGene.repairer);
            Assert.AreEqual(geneStr
                    .Replace(@"],""Weights"":", @",0.0],""Weights"":")
                    .Replace("],[", ",0.0,0.0],[")
                    .Replace("0.541667]]}",
                        "0.541667,0.0,0.0],[0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0]]}"),
                JsonConvert.SerializeObject(repairedGene.denseLayer1));

            // simpleGeneticBrain1Gene.repairer = repairedGene.repairer = null;
            simpleGeneticBrain1Gene.denseLayer1 = repairedGene.denseLayer1 = null;
            Assert.AreEqual(JsonConvert.SerializeObject(simpleGeneticBrain1Gene),
                JsonConvert.SerializeObject(repairedGene));
        }
    }
}