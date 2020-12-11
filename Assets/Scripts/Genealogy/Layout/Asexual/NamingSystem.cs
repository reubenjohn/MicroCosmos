using Genealogy.Graph;
using JetBrains.Annotations;

namespace Genealogy.Layout.Asexual
{
    public static class NamingSystem
    {
        public static string GetChildName([NotNull] GenealogyGraph tree, [NotNull] CellNode parent)
        {
            var nExistingSiblings = tree.GetRelationsFrom(parent.Guid)?.Count ?? 0;
            return $"{parent.displayName}.{nExistingSiblings + 1}";
        }
    }
}