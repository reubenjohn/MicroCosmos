using Genetics;
using Newtonsoft.Json;

namespace Brains.HunterBrain
{
    [JsonConverter(typeof(HunterBrainGeneConverter))]
    public class HunterBrainGene : IRepairableGene<HunterBrainGene, HunterBrainDescription>
    {
        [JsonIgnore] public readonly GeneRepairer<HunterBrainGene, HunterBrainDescription> repairer;


        public HunterBrainGene(GeneRepairer<HunterBrainGene, HunterBrainDescription> repairer)
        {
            this.repairer = repairer;
        }

        public HunterBrainGene RepairGene(HunterBrainDescription livingDescription) =>
            repairer.Invoke(this, livingDescription);
    }
}