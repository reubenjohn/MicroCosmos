using System;
using Genetics;
using UnityEngine;

namespace Organelles.Flagella
{
    public class FlagellaActuator : AbstractLivingComponent<FlagellaGene>, IActuator
    {
        public static readonly string ResourcePath = "Organelles/Flagella1";
        public static readonly string ActuatorType = typeof(FlagellaActuator).FullName;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private Cell.Cell cell;
        private Rigidbody2D rb { get; set; }

        private void Start()
        {
            cell = GetComponentInParent<Cell.Cell>();
            rb = GetComponentInParent<Rigidbody2D>();
        }

        public string GetActuatorType() => ActuatorType;

        public float[] Connect() => new float[2];

        public void Actuate(float[] logits)
        {
            if (cell.IsInFocus)
            {
                Grapher.Log(logits[0], "Flagella[0]", Color.blue);
                Grapher.Log(logits[1], "Flagella[1]", Color.cyan);
            }

            rb.AddRelativeForce(CalculateRelativeForce(gene, logits, rb.mass, Time.deltaTime));
            rb.AddTorque(CalculateTorque(gene, logits, rb.inertia, Time.deltaTime));
        }

        public override Transform OnInheritGene(FlagellaGene inheritedGene)
        {
            var angPow01 = Mathf.Clamp01(Mathf.Log10(inheritedGene.angularPower) - 2);
            GetComponent<Animator>().SetFloat(Speed, angPow01);
            foreach (var rend in GetComponentsInChildren<SpriteRenderer>())
            {
                Color.RGBToHSV(rend.color, out var hue, out var sat, out var value);
                var linPow01 = Mathf.Clamp01((Mathf.Log10(inheritedGene.linearPower) - 1) / 2f);
                rend.color = Color.HSVToRGB(hue, sat, linPow01);
            }

            return base.OnInheritGene(inheritedGene);
        }

        public static Vector2 CalculateRelativeForce(FlagellaGene gene, float[] logits, float mass, float deltaTime)
        {
            var forceMagnitude = logits[0] * gene.linearPower * mass * deltaTime;
            if (float.IsNaN(forceMagnitude))
                throw new InvalidOperationException($"Invalid force magnitude {forceMagnitude} resulting from: " +
                                                    $"power={gene.linearPower}, logit={logits[0]}, mass={mass}, deltaTime={deltaTime}");
            return forceMagnitude * Vector2.up;
        }

        public static float CalculateTorque(FlagellaGene gene, float[] logits, float inertia, float deltaTime)
        {
            var torque = logits[1] * gene.angularPower * inertia * deltaTime;
            if (float.IsNaN(torque))
                throw new InvalidOperationException($"Invalid force magnitude {torque} resulting from: " +
                                                    $"power={gene.angularPower}, logit={logits[1]}, inertia={inertia}, deltaTime={deltaTime}");
            return torque;
        }

        public override GeneTranscriber<FlagellaGene> GetGeneTranscriber() => FlagellaGeneTranscriber.Singleton;

        public override string GetResourcePath() => ResourcePath;
    }
}