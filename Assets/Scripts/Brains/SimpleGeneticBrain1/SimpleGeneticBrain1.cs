using System.Linq;
using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Util;

namespace Brains.SimpleGeneticBrain1
{
    public class SimpleGeneticBrain1 : AbstractBrain, ILivingComponent<SimpleGeneticBrain1Gene>
    {
        private SimpleGeneticBrain1Gene gene;

        private NeuralInterface neuralInterface;

        private new void Start()
        {
            base.Start();
            if (gene == null)
            {
                var inputLength = sensorLogits.Select(logits => logits.Length).Sum();
                var outputLength = actuatorLogits.Select(logits => logits.Length).Sum();
                gene = new SimpleGeneticBrain1Gene
                {
                    biases = RandomUtils.RandomLogits(outputLength),
                    weights = RandomUtils.RandomLogits(outputLength, inputLength)
                };
            }

            neuralInterface = new NeuralInterface(sensorLogits, actuatorLogits,
                new SimpleNeuralNetwork1(gene));
        }

        public string GetNodeName() => gameObject.name;

        Transform ILivingComponent.OnInheritGene(object inheritedGene) =>
            OnInheritGene((SimpleGeneticBrain1Gene) inheritedGene);

        public GeneTranscriber<SimpleGeneticBrain1Gene> GetGeneTranscriber() =>
            SimpleGeneticBrain1GeneTranscriber.Singleton;

        public SimpleGeneticBrain1Gene GetGene() => gene;

        public Transform OnInheritGene(SimpleGeneticBrain1Gene inheritedGene)
        {
            gene = inheritedGene;
            return transform;
        }

        IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();

        object ILivingComponent.GetGene() => GetGene();

        public string GetResourcePath() => "Organelles/GeneticBrain1";

        public JObject GetState() => new JObject();

        public void SetState(JObject state)
        {
        }

        public ILivingComponent[] GetSubLivingComponents()
        {
            return new ILivingComponent[] { };
        }

        protected override void React()
        {
            neuralInterface.React();
        }
    }
}