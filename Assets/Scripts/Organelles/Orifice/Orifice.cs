using System;
using Genetics;
using Structural;
using UnityEngine;

namespace Organelles.Orifice
{
    public class Orifice : AbstractLivingComponent<OrificeGene>, IActuator
    {
        public const string ResourcePath = "Organelles/Orifice1";
        public static readonly string ActuatorType = typeof(Orifice).FullName;

        public ContactFilter2D contactFilter;
        private readonly Collider2D[] collidersInRange = new Collider2D[1];

        private CellCauldron.CellCauldron cauldron;
        private Collider2D orificeCollider;
        private SpriteRenderer orificeSprite;


        private void Awake() => orificeSprite = GetComponentInChildren<SpriteRenderer>();

        private void Start()
        {
            cauldron = GetComponentInParent<CellCauldron.CellCauldron>();
            orificeCollider = GetComponentInChildren<Collider2D>();
        }

        public void Actuate(float[] logits)
        {
            var transferRate = cauldron.TotalMass * gene.transferRate * logits[0];
            if (transferRate == 0 ||
                Physics2D.OverlapCollider(orificeCollider, contactFilter, collidersInRange) <= 0) return;

            foreach (var coll in collidersInRange)
                if (coll.CompareTag("Cell"))
                {
                    var otherCauldron = coll.GetComponentInParent<CellCauldron.CellCauldron>();
                    var otherMass = otherCauldron.TotalMass;
                    var actualMassToTransfer = Math.Sign(transferRate) * Math.Min(Math.Abs(transferRate), otherMass);
                    var (src, dst) = actualMassToTransfer >= 0 ? (otherCauldron, cauldron) : (cauldron, otherCauldron);
                    src.TransferTo(dst, src.ToMixture() * (actualMassToTransfer / otherMass));
                }
        }

        public string GetActuatorType() => ActuatorType;

        public float[] Connect() => new float[1];

        public override Transform OnInheritGene(OrificeGene inheritedGene)
        {
            transform.localScale = inheritedGene.size * Vector2.one;
            orificeSprite.color = Color.Lerp(Color.gray, orificeSprite.color,
                OrificeGeneTranscriber.NormalizedTransferRate(inheritedGene.transferRate));

            GetComponentInParent<Membrane.Membrane>()
                .Attach(new CircularAttachment(
                    transform,
                    inheritedGene.circularMembraneAttachment
                ));
            return base.OnInheritGene(inheritedGene);
        }

        public override GeneTranscriber<OrificeGene> GetGeneTranscriber() => OrificeGeneTranscriber.Singleton;

        public override string GetResourcePath() => ResourcePath;
    }
}