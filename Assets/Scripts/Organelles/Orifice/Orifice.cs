using Environment;
using Genetics;
using UnityEngine;

namespace Organelles.Orifice
{
    public class Orifice : AbstractLivingComponent<OrificeGene>, IActuator
    {
        public static readonly string ActuatorType = typeof(Orifice).FullName;
        public static readonly string ResourcePath = "Organelles/Orifice1";

        public ContactFilter2D contactFilter;
        private readonly Collider2D[] collidersInRange = new Collider2D[1];

        private CellCauldron.CellCauldron cauldron;
        private Collider2D orificeCollider;


        private void Start()
        {
            cauldron = GetComponentInParent<CellCauldron.CellCauldron>();
            orificeCollider = GetComponent<Collider2D>();
        }

        public void Actuate(float[] logits)
        {
            var ingest = logits[0];
            if (ingest > 0)
                if (Physics2D.OverlapCollider(orificeCollider, contactFilter, collidersInRange) > 0)
                    foreach (var coll in collidersInRange)
                        if (coll.CompareTag("ChemicalBlob"))
                        {
                            var blob = coll.GetComponent<ChemicalBlob>();
                            var blobMass = blob.TotalMass;
                            if (blobMass > 0)
                            {
                                var maxTransfer = ingest * gene.transferRate;
                                var transferRatio = Mathf.Min(blobMass, maxTransfer) / blobMass;
                                blob.TransferTo(cauldron, blob.ToMixture() * transferRatio);
                            }
                            else
                            {
                                Destroy(coll.gameObject);
                            }
                        }
        }

        public string GetActuatorType() => ActuatorType;

        public float[] Connect() => new float[1];

        public override Transform OnInheritGene(OrificeGene inheritedGene)
        {
            transform.localScale = inheritedGene.size * Vector2.one;
            return base.OnInheritGene(inheritedGene);
        }

        public override GeneTranscriber<OrificeGene> GetGeneTranscriber() => OrificeGeneTranscriber.Singleton;

        public override string GetResourcePath() => ResourcePath;
    }
}