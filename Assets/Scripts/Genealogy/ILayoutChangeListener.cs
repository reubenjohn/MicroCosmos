using System.Collections.Generic;

namespace Genealogy
{
    public interface ILayoutChangeListener<in T>
    {
        void OnUpdateNode(T layout);
        void OnAddNode(T layout);
        void OnAddConnections(List<Relation> relations);
    }
}