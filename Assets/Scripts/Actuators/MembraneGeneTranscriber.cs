using Genetics;
using Newtonsoft.Json.Linq;

namespace Actuators
{
    public class MembraneGeneTranscriber : GeneTranscriber<MembraneGene>
    {
        public static readonly MembraneGeneTranscriber Singleton = new MembraneGeneTranscriber();
        public override MembraneGene Deserialize(JToken gene) => gene.ToObject<MembraneGene>();

        public override MembraneGene Mutate(MembraneGene gene) => new MembraneGene()
        {
            radius = gene.radius.MutateClamped(gene.radius * .1f, .1f, 5f)
        };
    }
}