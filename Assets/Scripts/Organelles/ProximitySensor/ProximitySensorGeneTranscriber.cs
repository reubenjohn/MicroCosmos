using Genetics;
using Newtonsoft.Json.Linq;

namespace Organelles.ProximitySensor
{
    public class ProximitySensorGeneTranscriber : GeneTranscriber<ProximitySensorGene>
    {
        public static readonly GeneTranscriber<ProximitySensorGene> Singleton = new ProximitySensorGeneTranscriber();

        private ProximitySensorGeneTranscriber() { }

        public override ProximitySensorGene Sample() => new ProximitySensorGene();

        public override ProximitySensorGene Deserialize(JToken gene) => gene.ToObject<ProximitySensorGene>();

        public override ProximitySensorGene Mutate(ProximitySensorGene gene) => new ProximitySensorGene();
    }
}