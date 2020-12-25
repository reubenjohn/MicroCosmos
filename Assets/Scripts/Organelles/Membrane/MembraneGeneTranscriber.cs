using System.Collections.Generic;
using Genetics;
using Newtonsoft.Json.Linq;
using Organelles.SimpleContainment;

namespace Organelles.Membrane
{
    public class MembraneGeneTranscriber : AbstractContainmentGeneTranscriber<MembraneGene>
    {
        public static readonly MembraneGeneTranscriber Singleton = new MembraneGeneTranscriber();

        private static readonly SubOrganelleCountsSimpleMutator SubOrganelleCountsMutator =
            new SubOrganelleCountsSimpleMutator(new Dictionary<string, GeneMutator<float>>
            {
                {BirthCanal.BirthCanal.ResourcePath, x => x.MutateClamped(.01f, .6f, .99f)}
            });

        private MembraneGeneTranscriber() { }

        public override MembraneGene Sample() =>
            new MembraneGene
            {
                radius = .25f,
                nSubOrganelles = SubOrganelleCountsMutator.Mutate(new SubOrganelleCounts())
            };

        public override MembraneGene Deserialize(JToken gene) => gene.ToObject<MembraneGene>();

        public override MembraneGene Mutate(MembraneGene gene) =>
            new MembraneGene
            {
                radius = gene.radius,
                nSubOrganelles = SubOrganelleCountsMutator.Mutate(gene.nSubOrganelles)
            };
    }
}