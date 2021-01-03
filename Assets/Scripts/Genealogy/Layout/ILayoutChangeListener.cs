using System.Collections.Generic;
using Genealogy.Graph;

namespace Genealogy.Layout
{
    public interface ILayoutChangeListener<in T>
    {
        void OnUpdateNode(T layout);
        void OnAddNode(T layout);
        void OnRemoveNode(T layout);
        void OnAddConnections(List<Relation> relations);
        void OnClear();
    }
}