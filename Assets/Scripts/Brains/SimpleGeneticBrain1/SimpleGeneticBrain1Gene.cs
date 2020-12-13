using Genetics;
using Newtonsoft.Json;

namespace Brains.SimpleGeneticBrain1
{
    [JsonConverter(typeof(SimpleGeneticBrain1GeneConverter))]
    public class SimpleGeneticBrain1Gene : IRepairableGene<SimpleGeneticBrain1Gene, SimpleGeneticBrain1Description>
    {
        [JsonIgnore] public readonly GeneRepairer<SimpleGeneticBrain1Gene, SimpleGeneticBrain1Description> repairer;

        public DenseLayerGene denseLayer1;

        public SimpleGeneticBrain1Gene(GeneRepairer<SimpleGeneticBrain1Gene, SimpleGeneticBrain1Description> repairer)
        {
            this.repairer = repairer;
        }

        public SimpleGeneticBrain1Gene RepairGene(SimpleGeneticBrain1Description livingDescription) =>
            repairer.Invoke(this, livingDescription);
    }
}