using System.Collections.Generic;
using Genealogy.AsexualFamilyTree;

namespace Genealogy
{
    public interface ILayoutChangeListener<in T>
    {
        void OnUpdateNode(T layout);
        void OnAddNode(T layout);
    }
}