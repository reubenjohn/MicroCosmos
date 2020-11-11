using System.Linq;
using DefaultNamespace;
using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Brains
{
    public class SimpleGeneticBrain : AbstractBrain, ILivingComponent<GeneticBrainGene>
    {
        private GeneticBrainGene gene;

        private float[] flattenedInput;
        private float[] flattenedOutput;
        private FullyConnectedLayer dense1;

        private new void Start()
        {
            base.Start();
            flattenedInput = new float[sensorLogits.Select(logits => logits.Length).Sum()];
            flattenedOutput = new float[actuatorLogits.Select(logits => logits.Length).Sum()];
            if (gene == null)
            {
                gene = new GeneticBrainGene()
                {
                    biases = RandomUtils.RandomLogits(flattenedOutput.Length),
                    weights = RandomUtils.RandomLogits(flattenedOutput.Length, flattenedInput.Length)
                };
            }
        }

        public override void React(float[][] sensoryLogits)
        {
            Logits.Flatten(sensoryLogits, flattenedInput);

            dense1.Calculate(flattenedInput, flattenedOutput);

            Logits.Unflatten(flattenedOutput, actuatorLogits);
        }

        public string GetNodeName() => gameObject.name;

        Transform ILivingComponent.OnInheritGene(object inheritedGene) =>
            OnInheritGene((GeneticBrainGene) inheritedGene);

        public GeneTranscriber<GeneticBrainGene> GetGeneTranscriber() => GeneticBrainGeneTranscriber.Singleton;

        public GeneticBrainGene GetGene() => gene;

        public Transform OnInheritGene(GeneticBrainGene inheritedGene)
        {
            gene = inheritedGene;
            var weights = inheritedGene?.weights ?? new float[flattenedOutput.Length, flattenedInput.Length]
                .MutateClamped(1f, -1f, 1f);
            var biases = inheritedGene?.biases ??
                         new float[flattenedOutput.Length]
                             .Select(b => b.MutateClamped(2f, -1f, 1f))
                             .ToArray();
            dense1 = new FullyConnectedLayer(weights, biases);
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