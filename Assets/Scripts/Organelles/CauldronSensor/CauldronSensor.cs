using System;
using ChemistryMicro;
using Genetics;
using UnityEngine;
using Util;

namespace Organelles.CauldronSensor
{
    public class CauldronSensor : AbstractLivingComponent<CauldronSensorGene>, ISensor
    {
        public static readonly string ResourcePath = "Organelles/CauldronSensor1";

        private CellCauldron.CellCauldron cauldron;

        private void Start()
        {
            cauldron = GetComponentInParent<Cell.Cell>().Cauldron;
        }

        public float[] Connect() => new float[1 + EnumUtils.EnumCount(typeof(Substance))];

        public void Sense(float[] logits)
        {
            for (var i = 0; i < logits.Length; i++)
                logits[i] = 0;

            var totalMass = cauldron.TotalMass;

            var nUsedLogits = 0;
            logits[nUsedLogits++] = Mathf.Clamp((Mathf.Log10(totalMass) + 1) / 2, -1, 1);

            foreach (var substance in Enum.GetValues(typeof(Substance)))
            {
                var relativeMass = Mathf.Clamp(cauldron[(Substance) substance] / totalMass, 0f, float.MaxValue);
                relativeMass = float.IsNaN(relativeMass) ? 0f : relativeMass;
                var logRelativeMass = Mathf.Log10(relativeMass);
                logits[nUsedLogits++] = Mathf.Clamp(logRelativeMass, -1f, 1f);
            }
        }

        public override GeneTranscriber<CauldronSensorGene> GetGeneTranscriber() =>
            CauldronSensorGeneTranscriber.Singleton;

        public override string GetResourcePath() => ResourcePath;
    }
}