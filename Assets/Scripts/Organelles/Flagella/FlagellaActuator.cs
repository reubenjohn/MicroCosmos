using Genetics;
using UnityEngine;

namespace Organelles.Flagella
{
    public class FlagellaActuator : AbstractLivingComponent<FlagellaGene>, IActuator
    {
        public static readonly string ResourcePath = "Organelles/Flagella1";
        private Cell.Cell cell;
        private Rigidbody2D rb { get; set; }

        private void Start()
        {
            cell = GetComponentInParent<Cell.Cell>();
            rb = GetComponentInParent<Rigidbody2D>();
            gene = gene ?? new FlagellaGene(250f, 10f);
        }

        public float[] Connect() => new float[2];

        public void Actuate(float[] logits)
        {
            if (cell.IsInFocus)
            {
                Grapher.Log(logits[0], "Flagella[0]", Color.blue);
                Grapher.Log(logits[1], "Flagella[1]", Color.cyan);
            }

            rb.AddRelativeForce(CalculateRelativeForce(gene, logits, Time.deltaTime));
            rb.AddTorque(CalculateTorque(gene, logits, Time.deltaTime));
        }

        public static Vector2 CalculateRelativeForce(FlagellaGene gene, float[] logits, float deltaTime) =>
            logits[0] * gene.linearPower * deltaTime * Vector2.up;

        public static float CalculateTorque(FlagellaGene gene, float[] logits, float deltaTime) =>
            logits[1] * gene.angularPower * deltaTime;

        public override GeneTranscriber<FlagellaGene> GetGeneTranscriber() => FlagellaGeneTranscriber.Singleton;

        public override string GetResourcePath() => ResourcePath;
    }
}