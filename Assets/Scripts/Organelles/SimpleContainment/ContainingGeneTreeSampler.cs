using System.Linq;
using Genetics;
using Persistence;

namespace Organelles.SimpleContainment
{
    public class ContainingGeneTreeSampler : DefaultGeneTreeSampler
    {
        public new static readonly ContainingGeneTreeSampler Singleton = new ContainingGeneTreeSampler();
        private ContainingGeneTreeSampler() { }

        public override GeneNode Sample(ILivingComponent livingComponent)
        {
            var tree = base.Sample(livingComponent);
            var gene = (ContainmentGene) tree.gene;
            tree.children = ContainedOrganelleMutator.GetMutatedChildren(gene.nSubOrganelles, new GeneNode[0])
                .ToArray();
            return tree;
        }
    }
}