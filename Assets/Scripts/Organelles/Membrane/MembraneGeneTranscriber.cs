using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Organelles.SimpleContainment;

namespace Organelles.Membrane
{
    public class MembraneGeneTranscriber : AbstractContainmentGeneTranscriber<MembraneGene>
    {
        public static readonly MembraneGeneTranscriber Singleton = new MembraneGeneTranscriber();

        [JsonIgnore] private static readonly string[] SupportedSubLivingComponentsResources =
            {BirthCanal.BirthCanal.ResourcePath};

        private MembraneGeneTranscriber() { }

        public override MembraneGene Sample() =>
            new MembraneGene
            {
                radius = .25f,
                nSubOrganelles = new SubOrganelleCounts(SupportedSubLivingComponentsResources)
            };

        public override MembraneGene Deserialize(JToken gene) => gene.ToObject<MembraneGene>();

        public override MembraneGene Mutate(MembraneGene gene) =>
            new MembraneGene
            {
                radius = gene.radius,
                nSubOrganelles = gene.nSubOrganelles.Mutate(.1f)
            };
    }
}