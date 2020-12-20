using System.Linq;
using Brains.SimpleGeneticBrain1;
using Genetics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        [JsonIgnore] private static readonly string[] SupportedSubLivingComponentsResources =
        {
            FlagellaActuator.ResourcePath,
            ProximitySensor.ResourcePath
        };

        private CellGeneTranscriber() { }


        public override CellGene Sample() =>
            new CellGene
            {
                cauldron = CellCauldron.SampleGene(),
                nSubOrganelles = new SubOrganelleCounts(SupportedSubLivingComponentsResources)
                {
                    {Membrane.ResourcePath, .6f}, // Exactly one membrane
                    {SimpleGeneticBrain1.ResourcePath, .6f} // Exactly one brain
                }
            };

        public override CellGene Deserialize(JToken gene) => gene.ToObject<CellGene>();

        public override CellGene Mutate(CellGene gene)
        {
            var subOrganelleCounts = gene.nSubOrganelles.Mutate(.1f);
            subOrganelleCounts[Membrane.ResourcePath] = .6f;
            subOrganelleCounts[SimpleGeneticBrain1.ResourcePath] = .6f;
            return new CellGene
            {
                cauldron = Mutate(gene.cauldron),
                nSubOrganelles = subOrganelleCounts
            };
        }

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