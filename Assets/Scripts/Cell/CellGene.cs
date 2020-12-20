using System;
using Organelles.CellCauldron;
using Organelles.SimpleContainment;

namespace Cell
{
    [Serializable]
    public class CellGene : ContainmentGene
    {
        //TODO Make Constructor
        public ChemicalBagGene cauldron;
    }
}