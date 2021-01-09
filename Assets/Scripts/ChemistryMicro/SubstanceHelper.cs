using System;
using System.Linq;
using Util;

namespace ChemistryMicro
{
    public static class SubstanceHelper
    {
        public static readonly int NSubstances;
        public static readonly Substance[] Substances;

        static SubstanceHelper()
        {
            NSubstances = EnumUtils.Count(typeof(Substance));
            Substances = Enum.GetValues(typeof(Substance)).Cast<Substance>().ToArray();
        }
    }
}