using System.Collections.Generic;
using System.Linq;
using Genetics;
using Persistence;

namespace Organelles.SimpleContainment
{
    public static class ContainedOrganelleMutator
    {
        public static GeneNode[] GetMutatedChildren(GeneNode node) =>
            GetMutatedChildren(((ContainmentGene) node.gene).nSubOrganelles, node.children)
                .ToArray();

        public static IEnumerable<GeneNode> GetMutatedChildren(SubOrganelleCounts newCounts,
            GeneNode[] existingChildren)
        {
            var groups = existingChildren.GroupBy(node => node.resource)
                .ToDictionary(
                    group => group.Key,
                    group => group.ToArray()
                );
            foreach (var resource in newCounts.Keys)
            {
                if (!groups.TryGetValue(resource, out var existingOrganelles))
                    existingOrganelles = new GeneNode[0];
                var existingCount = existingOrganelles.Length;
                var newCount = newCounts.GetCount(resource);
                for (var i = 0; i < newCount; i++)
                {
                    GeneNode childNode;
                    ILivingComponent livingComponent;
                    if (i < existingCount)
                    {
                        childNode = existingOrganelles[i];
                        livingComponent = childNode.livingComponent;
                    }
                    else
                    {
                        livingComponent = LivingComponentRegistry.Get(resource);
                        childNode = livingComponent.GetGeneTranscriber().GetTreeSampler().Sample(livingComponent);
                    }

                    yield return livingComponent.GetGeneTranscriber().GetTreeMutator().GetMutated(childNode);
                }
            }
        }
    }
}