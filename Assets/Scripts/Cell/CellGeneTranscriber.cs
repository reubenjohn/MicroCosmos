using Newtonsoft.Json.Linq;

public class CellGeneTranscriber : GeneTranscriber<CellGene>
{
    public static readonly CellGeneTranscriber Singleton = new CellGeneTranscriber();

    private CellGeneTranscriber()
    {
    }

    public override CellGene Deserialize(JToken gene) => gene.ToObject<CellGene>();

    public override CellGene Mutate(CellGene gene) => new CellGene();
}