using Newtonsoft.Json.Linq;

namespace Genetics
{
    public abstract class RepairableGeneTranscriber<TGene, TDescription> : GeneTranscriber<TGene>
        where TGene : IRepairableGene<TGene, TDescription>
    {
        public abstract override TGene Sample();
        public abstract GeneRepairer<TGene, TDescription> GetRepairer();
        public abstract override TGene Deserialize(JToken gene);
        public abstract override TGene Mutate(TGene gene);
    }
}