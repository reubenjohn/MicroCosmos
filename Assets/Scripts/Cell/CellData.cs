using System;
using Persistence;

namespace Cell
{
    [Serializable]
    public class CellData
    {
        public GeneNode geneTree { get; set; }
        public StateNode stateTree { get; set; }
    }
}