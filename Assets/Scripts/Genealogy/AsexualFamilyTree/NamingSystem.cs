using JetBrains.Annotations;

namespace Genealogy.AsexualFamilyTree
{
    public class NamingSystem
    {
        public static string GetChildName([NotNull] FamilyTree tree, [NotNull] CellNode parent)
        {
            var nExistingSiblings = tree.GetRelationsFrom(parent.Guid)?.Count ?? 0;
            return $"{parent.displayName}.{nExistingSiblings + 1}";
        }
    }
}