using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

public class CellGeneTranscriber : IGeneTranscriber<CellGene>
{
    public static readonly CellGeneTranscriber SINGLETON = new CellGeneTranscriber();

    private CellGeneTranscriber() { }

    public override CellGene Deserialize(JToken gene) => gene.ToObject<CellGene>();

    public override CellGene Mutate(CellGene gene) => new CellGene();
}
