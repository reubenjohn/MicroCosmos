namespace Genetics
{
    public delegate TG GeneRepairer<TG, in TS>(TG gene, TS expressedDescription);

    public interface IRepairableGene<out TGene, in TDescription> where TGene : IRepairableGene<TGene, TDescription>
    {
        TGene RepairGene(TDescription livingDescription);
    }
}