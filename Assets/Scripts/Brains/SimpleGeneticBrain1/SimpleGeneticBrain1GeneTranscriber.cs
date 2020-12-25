using System;
using System.Linq;
using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Util;

namespace Brains.SimpleGeneticBrain1
{
    public class SimpleGeneticBrain1GeneTranscriber :
        RepairableGeneTranscriber<SimpleGeneticBrain1Gene, SimpleGeneticBrain1Description>
    {
        public static readonly SimpleGeneticBrain1GeneTranscriber Singleton = new SimpleGeneticBrain1GeneTranscriber();

        public static readonly GeneRepairer<SimpleGeneticBrain1Gene, SimpleGeneticBrain1Description>
            Repairer = RepairGene;

        public override SimpleGeneticBrain1Gene Sample() =>
            new SimpleGeneticBrain1Gene(Repairer)
            {
                denseLayer1 = new DenseLayerGene(
                    RandomUtils.RandomLogits(2, 3),
                    RandomUtils.RandomLogits(2)
                )
            };

        public override GeneRepairer<SimpleGeneticBrain1Gene, SimpleGeneticBrain1Description> GetRepairer() => Repairer;

        private static SimpleGeneticBrain1Gene RepairGene(SimpleGeneticBrain1Gene gene,
            SimpleGeneticBrain1Description expressedDescription) =>
            new SimpleGeneticBrain1Gene(Repairer)
            {
                denseLayer1 = RepairDenseLayerGene(gene.denseLayer1, expressedDescription.DenseLayer1)
            };

        private static DenseLayerGene RepairDenseLayerGene(DenseLayerGene gene,
            DenseLayerInterfaceDescription interfaceDescription)
        {
            var oldWeightsLength1 = gene.Weights.GetLength(0);
            var oldWeightsLength2 = gene.Weights.GetLength(1);
            var newWeightsLength1 = Mathf.Max(oldWeightsLength1, interfaceDescription.OutputLength);
            var newWeightsLength2 = Mathf.Max(oldWeightsLength2, interfaceDescription.InputLength);
            var oldBiasesLength = gene.Biases.Length;
            var newBiasesLength = Mathf.Max(gene.Biases.Length, interfaceDescription.OutputLength);

            if (interfaceDescription.InputLength <= oldWeightsLength1 &&
                interfaceDescription.OutputLength <= oldBiasesLength)
                return gene;

            var weights = new float[newWeightsLength1, newWeightsLength2];
            var biases = new float[newBiasesLength];
            ArrayUtils.Copy(gene.Weights, weights, oldWeightsLength1, oldWeightsLength2);
            RandomUtils.RandomizeLogits(weights, newWeightsLength1, newWeightsLength2,
                oldWeightsLength1, oldWeightsLength2);
            Array.Copy(gene.Biases, biases, oldBiasesLength);
            RandomUtils.RandomizeLogits(biases, newBiasesLength, oldBiasesLength);
            return new DenseLayerGene(weights, biases);
        }

        public override SimpleGeneticBrain1Gene Deserialize(JToken geneToken) =>
            geneToken.ToObject<SimpleGeneticBrain1Gene>();

        public override SimpleGeneticBrain1Gene Mutate(SimpleGeneticBrain1Gene gene) =>
            new SimpleGeneticBrain1Gene(Repairer)
            {
                denseLayer1 = new DenseLayerGene(
                    gene.denseLayer1.Weights.MutateClamped(.05f, -1f, 1f),
                    gene.denseLayer1.Biases.Select(bias => bias.MutateClamped(.05f, -1f, 1f)).ToArray()
                )
            };
    }
}