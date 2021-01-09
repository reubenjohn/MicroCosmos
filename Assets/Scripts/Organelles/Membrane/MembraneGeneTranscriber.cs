using System.Collections.Generic;
using Genetics;
using Newtonsoft.Json.Linq;
using Organelles.SimpleContainment;
using UnityEngine;

namespace Organelles.Membrane
{
    public class MembraneGeneTranscriber : AbstractContainmentGeneTranscriber<MembraneGene>
    {
        public static readonly MembraneGeneTranscriber Singleton = new MembraneGeneTranscriber();

        private static readonly SubOrganelleCountsSimpleMutator SubOrganelleCountsMutator =
            new SubOrganelleCountsSimpleMutator(new Dictionary<string, GeneMutator<float>>
            {
                {BirthCanal.BirthCanal.ResourcePath, x => x.MutateClamped(.01f, .6f, .99f)},
                {Orifice.Orifice.ResourcePath, x => .6f}
            });

        private MembraneGeneTranscriber() { }

        public override MembraneGene Sample() =>
            new MembraneGene
            {
                radius = .25f,
                relativeThickness = Random.Range(.1f, .5f),
                nSubOrganelles = SubOrganelleCountsMutator.Mutate(new SubOrganelleCounts())
            };

        public override MembraneGene Deserialize(JToken gene) => gene.ToObject<MembraneGene>();

        public override MembraneGene Mutate(MembraneGene gene) =>
            new MembraneGene
            {
                radius = gene.radius,
                relativeThickness = gene.relativeThickness.MutateClamped(gene.relativeThickness * .01f, .05f, .9f),
                nSubOrganelles = SubOrganelleCountsMutator.Mutate(gene.nSubOrganelles)
            };
    }
}