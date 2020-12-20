using System.Linq;

namespace Genetics
{
    public static class DefaultGeneTreeChildrenMutator
    {
        public static readonly GeneTreeChildrenMutator Singleton = parentNode => parentNode
            .children
            .Select(node => node.livingComponent.GetGeneTranscriber().GetTreeMutator().GetMutated(node))
            .ToArray();
    }
}