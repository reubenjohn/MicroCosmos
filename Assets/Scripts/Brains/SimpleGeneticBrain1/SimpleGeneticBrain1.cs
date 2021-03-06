﻿using System.Linq;
using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Brains.SimpleGeneticBrain1
{
    public class SimpleGeneticBrain1 : AbstractBrain, ILivingComponent<SimpleGeneticBrain1Gene>
    {
        public const string ResourcePath = "Organelles/GeneticBrain1";
        private SimpleGeneticBrain1Gene gene;

        private NeuralInterface neuralInterface;

        protected override void Start()
        {
            base.Start();
            ResetBrain();
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

        public string GetResourcePath() => ResourcePath;

        public JObject GetState() => new JObject();

        public void SetState(JObject state) { }

        public ILivingComponent[] GetSubLivingComponents() => new ILivingComponent[] { };

        private void ResetBrain()
        {
            var livingDescription = new SimpleGeneticBrain1Description(
                sensorLogits.Select(logits => logits.Length).Sum(),
                actuatorLogits.Select(logits => logits.Length).Sum()
            );
            if (gene is IRepairableGene<SimpleGeneticBrain1Gene, SimpleGeneticBrain1Description> repairableGene)
                gene = repairableGene.RepairGene(livingDescription);

            neuralInterface = new NeuralInterface(sensorLogits, actuatorLogits, new SimpleNeuralNetwork1(gene));
        }

        protected override void React()
        {
            try
            {
                neuralInterface.React();
            }
            catch (InputSizeMismatchException)
            {
                ResetBrain();
                neuralInterface.React();
            }
        }
    }
}