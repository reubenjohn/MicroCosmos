using System;
using System.Linq;
using Genetics;
using Newtonsoft.Json.Linq;
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
                    RandomUtils.RandomLogits(8, 16),
                    RandomUtils.RandomLogits(8)
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
            if (interfaceDescription.InputLength <= gene.Weights.GetLength(0) &&
                interfaceDescription.OutputLength <= gene.Biases.Length)
                return gene;

            var weights = new float[interfaceDescription.OutputLength, interfaceDescription.InputLength];
            var biases = new float[interfaceDescription.OutputLength];
            ArrayUtils.Copy(gene.Weights, weights,
                gene.Weights.GetLength(0), gene.Weights.GetLength(1));
            Array.Copy(gene.Biases, biases, gene.Biases.Length);
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