using System.Collections.Generic;
using System.Linq;
using Brains.SimpleGeneticBrain1;
using Genetics;
using Newtonsoft.Json.Linq;
using Organelles.CauldronSensor;
using Organelles.CellCauldron;
using Organelles.Flagella;
using Organelles.Membrane;
using Organelles.ProximitySensor;
using Organelles.SimpleContainment;

namespace Cell
{
    public class CellGeneTranscriber : AbstractContainmentGeneTranscriber<CellGene>
    {
        public static readonly CellGeneTranscriber Singleton = new CellGeneTranscriber();

        private static readonly SubOrganelleCountsSimpleMutator SubOrganelleCountsMutator =
            new SubOrganelleCountsSimpleMutator(new Dictionary<string, GeneMutator<float>>
            {
                {Membrane.ResourcePath, x => .6f}, // Exactly one membrane
                {SimpleGeneticBrain1.ResourcePath, x => .6f}, // Exactly one brain
                {CauldronSensor.ResourcePath, x => .6f}, // Exactly one cauldron sensor
                {FlagellaActuator.ResourcePath, x => .6f}, // Exactly one flagella
                {ProximitySensor.ResourcePath, x => x.MutateClamped(.01f, 0f, .95f)}
            });

        private CellGeneTranscriber() { }


        public override CellGene Sample() =>
            new CellGene
            {
                cauldron = CellCauldron.SampleGene(),
                nSubOrganelles = SubOrganelleCountsMutator.Mutate(new SubOrganelleCounts())
            };

        public override CellGene Deserialize(JToken gene) => gene.ToObject<CellGene>();

        public override CellGene Mutate(CellGene gene) =>
            new CellGene
            {
                cauldron = Mutate(gene.cauldron),
                nSubOrganelles = SubOrganelleCountsMutator.Mutate(gene.nSubOrganelles)
            };

        private ChemicalBagGene Mutate(ChemicalBagGene gene)
        {
            return new ChemicalBagGene
            {
                initialCauldron = gene.initialCauldron.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.MutateClamped(pair.Value * .05f, 0, 100f)
                )
            };
        }
    }
}