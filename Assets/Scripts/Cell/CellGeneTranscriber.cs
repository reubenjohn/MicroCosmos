using System.Linq;
using Genetics;
using Newtonsoft.Json.Linq;
using Organelles.ChemicalBag;

namespace Cell
{
    public class CellGeneTranscriber : GeneTranscriber<CellGene>
    {
        public static readonly CellGeneTranscriber Singleton = new CellGeneTranscriber();

        private CellGeneTranscriber()
        {
        }

        public override CellGene Deserialize(JToken gene)
        {
            return gene.ToObject<CellGene>();
        }

        public override CellGene Mutate(CellGene gene)
        {
            return new CellGene
            {
                cauldron = Mutate(gene.cauldron)
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