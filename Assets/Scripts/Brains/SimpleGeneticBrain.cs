using System.Linq;
using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Brains
{
    public class SimpleGeneticBrain : AbstractBrain, ILivingComponent<GeneticBrainGene>
    {
        private GeneticBrainGene gene;

        private NeuralInterface neuralInterface;

        private new void Start()
        {
            base.Start();
            if (gene == null)
            {
                var inputLength = sensorLogits.Select(logits => logits.Length).Sum();
                var outputLength = actuatorLogits.Select(logits => logits.Length).Sum();
                gene = new GeneticBrainGene()
                {
                    biases = RandomUtils.RandomLogits(outputLength),
                    weights = RandomUtils.RandomLogits(outputLength, inputLength)
                };
            }

            neuralInterface = new NeuralInterface(sensorLogits, actuatorLogits,
                new SimpleNeuralNetwork1(gene));
        }

        protected override void React() => neuralInterface.React();

        public string GetNodeName() => gameObject.name;

        Transform ILivingComponent.OnInheritGene(object inheritedGene) =>
            OnInheritGene((GeneticBrainGene) inheritedGene);

        public GeneTranscriber<GeneticBrainGene> GetGeneTranscriber() => GeneticBrainGeneTranscriber.Singleton;

        public GeneticBrainGene GetGene() => gene;

        public Transform OnInheritGene(GeneticBrainGene inheritedGene)
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

        public ILivingComponent[] GetSubLivingComponents() => new ILivingComponent[] { };
    }
}