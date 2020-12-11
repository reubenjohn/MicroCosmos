using System.Linq;
using Genetics;
using Newtonsoft.Json.Linq;

namespace Brains.SimpleGeneticBrain1
{
    public class SimpleGeneticBrain1GeneTranscriber : GeneTranscriber<SimpleGeneticBrain1Gene>
    {
        public static readonly SimpleGeneticBrain1GeneTranscriber Singleton = new SimpleGeneticBrain1GeneTranscriber();

        public override SimpleGeneticBrain1Gene Deserialize(JToken gene)
        {
            return gene.ToObject<SimpleGeneticBrain1Gene>();
        }

        public override SimpleGeneticBrain1Gene Mutate(SimpleGeneticBrain1Gene gene)
        {
            return new SimpleGeneticBrain1Gene
            {
                biases = gene.biases.Select(bias => bias.MutateClamped(.05f, -1f, 1f)).ToArray(),
                weights = gene.weights.MutateClamped(.05f, -1f, 1f)
            };
        }
    }
}