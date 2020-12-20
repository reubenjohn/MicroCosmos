using Persistence;

namespace Genetics
{
    public class GeneTreeMutator
    {
        private readonly GeneMutator geneMutator;
        private readonly GeneTreeChildrenMutator geneTreeChildrenMutator;

        protected GeneTreeMutator(GeneMutator geneMutator, GeneTreeChildrenMutator geneTreeChildrenMutator)
        {
            this.geneMutator = geneMutator;
            this.geneTreeChildrenMutator = geneTreeChildrenMutator;
        }

        public GeneNode GetMutated(GeneNode geneNode) =>
            new GeneNode(
                geneNode.livingComponent,
                geneMutator(geneNode.gene),
                geneTreeChildrenMutator(geneNode)
            );
    }

    public class GeneTreeMutator<T> : GeneTreeMutator
    {
        public GeneTreeMutator(GeneMutator<T> geneMutator, GeneTreeChildrenMutator geneTreeChildrenMutator)
            : base(gene => geneMutator((T) gene), geneTreeChildrenMutator) { }
    }
}