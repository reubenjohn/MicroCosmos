using System.Linq;
using Genetics;
using Newtonsoft.Json.Linq;

namespace Brains
{
    public class GeneticBrainGeneTranscriber : GeneTranscriber<GeneticBrainGene>
    {
        public static readonly GeneticBrainGeneTranscriber Singleton = new GeneticBrainGeneTranscriber();

        public override GeneticBrainGene Deserialize(JToken gene) => gene.ToObject<GeneticBrainGene>();

        public override GeneticBrainGene Mutate(GeneticBrainGene gene) => new GeneticBrainGene()
        {
            biases = gene.biases.Select(bias => Mutator.MutateClamped((float) bias, .05f, -1f, 1f)).ToArray(),
            weights = gene.weights.MutateClamped(.05f, -1f, 1f),
        };
    }
}