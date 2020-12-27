using Genetics;
using Newtonsoft.Json.Linq;

namespace Organelles.CauldronSensor
{
    public class CauldronSensorGeneTranscriber : GeneTranscriber<CauldronSensorGene>
    {
        public static readonly GeneTranscriber<CauldronSensorGene> Singleton = new CauldronSensorGeneTranscriber();

        private CauldronSensorGeneTranscriber() { }

        public override CauldronSensorGene Sample() => new CauldronSensorGene();

        public override CauldronSensorGene Deserialize(JToken gene) => gene.ToObject<CauldronSensorGene>();

        public override CauldronSensorGene Mutate(CauldronSensorGene gene) => new CauldronSensorGene();
    }
}