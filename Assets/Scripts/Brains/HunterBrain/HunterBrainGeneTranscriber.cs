using System;
using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Util;

namespace Brains.HunterBrain
{
    public class HunterBrainGeneTranscriber :
        RepairableGeneTranscriber<HunterBrainGene, HunterBrainDescription>
    {
        public static readonly HunterBrainGeneTranscriber Singleton = new HunterBrainGeneTranscriber();

        public static readonly GeneRepairer<HunterBrainGene, HunterBrainDescription>
            Repairer = RepairGene;

        public override HunterBrainGene Sample() => new HunterBrainGene(Repairer);

        public override GeneRepairer<HunterBrainGene, HunterBrainDescription> GetRepairer() => Repairer;

        private static HunterBrainGene RepairGene(HunterBrainGene gene,
            HunterBrainDescription expressedDescription) =>
            new HunterBrainGene(Repairer);

        private static DenseLayerGene RepairDenseLayerGene(DenseLayerGene gene,
            DenseLayerInterfaceDescription interfaceDescription)
        {
            var oldWeightsLength1 = gene.Weights.GetLength(0);
            var oldWeightsLength2 = gene.Weights.GetLength(1);
            var newWeightsLength1 = Mathf.Max(oldWeightsLength1, interfaceDescription.OutputLength);
            var newWeightsLength2 = Mathf.Max(oldWeightsLength2, interfaceDescription.InputLength);
            var oldBiasesLength = gene.Biases.Length;
            var newBiasesLength = Mathf.Max(gene.Biases.Length, interfaceDescription.OutputLength);

            if (interfaceDescription.InputLength <= oldWeightsLength2 &&
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

        public override HunterBrainGene Deserialize(JToken geneToken) => geneToken.ToObject<HunterBrainGene>();

        public override HunterBrainGene Mutate(HunterBrainGene gene) => new HunterBrainGene(Repairer);
    }
}