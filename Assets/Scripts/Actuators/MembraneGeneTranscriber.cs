using Genetics;
using Newtonsoft.Json.Linq;

namespace Actuators
{
    public class MembraneGeneTranscriber : GeneTranscriber<MembraneGene>
    {
        public override MembraneGene Deserialize(JToken gene) => gene.ToObject<MembraneGene>();

        public override MembraneGene Mutate(MembraneGene gene) => new MembraneGene();
    }
}