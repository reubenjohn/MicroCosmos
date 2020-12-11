using Genetics;
using Newtonsoft.Json.Linq;

namespace Organelles.ProximitySensor
{
    public class ProximitySensorGeneTranscriber : GeneTranscriber<ProximitySensorGene>
    {
        public static readonly GeneTranscriber<ProximitySensorGene> Singleton = new ProximitySensorGeneTranscriber();

        public override ProximitySensorGene Deserialize(JToken gene)
        {
            return gene.ToObject<ProximitySensorGene>();
        }

        public override ProximitySensorGene Mutate(ProximitySensorGene gene)
        {
            return new ProximitySensorGene();
        }
    }
}