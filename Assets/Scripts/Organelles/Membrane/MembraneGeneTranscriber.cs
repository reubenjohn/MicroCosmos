using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Organelles.Membrane
{
    public class MembraneGeneTranscriber : GeneTranscriber<MembraneGene>
    {
        public static readonly MembraneGeneTranscriber Singleton = new MembraneGeneTranscriber();

        public override MembraneGene Sample() =>
            new MembraneGene
            {
                radius = Random.Range(.1f, 5f)
            };

        public override MembraneGene Deserialize(JToken gene)
        {
            return gene.ToObject<MembraneGene>();
        }

        public override MembraneGene Mutate(MembraneGene gene)
        {
            return new MembraneGene
            {
                radius = gene.radius.MutateClamped(gene.radius * .1f, .1f, 5f)
            };
        }
    }
}