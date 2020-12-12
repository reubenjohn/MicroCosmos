using System.Linq;
using ChemistryMicro;
using Genetics;
using Structural;
using UnityEngine;
using Util;

namespace Organelles.Membrane
{
    public class Membrane : AbstractLivingComponent<MembraneGene>
    {
        private static readonly string ResourcePath = "Organelles/Membrane1";
        private static readonly float MinMembraneThickness = .01f;
        private readonly CircularAttachmentRing attachmentAdapter = new CircularAttachmentRing();
        private CellCauldron.CellCauldron cauldron;
        private Cell.Cell cell;

        private float Thickness => Radius - InnerRadius;

        private float Radius
        {
            get => transform.localScale.x;
            set => transform.localScale = Vector3.one * value;
        }

        private float InnerRadius =>
            Mathf.Pow(Mathf.Max(0, Mathf.Pow(Radius, 3) - cauldron[Substance.Skin] / (4f / 3f * Mathf.PI)), 1f / 3);

        private void Start()
        {
            cell = GetComponentInParent<Cell.Cell>();
            cauldron = GetComponentInParent<CellCauldron.CellCauldron>();
        }

        private void Update()
        {
            var thickness = Thickness;
            if (cell.IsInFocus) Grapher.Log(thickness, "Membrane.Thickness");
            if (thickness < MinMembraneThickness)
                cell.Die();
        }

        public override GeneTranscriber<MembraneGene> GetGeneTranscriber() => MembraneGeneTranscriber.Singleton;

        public override string GetResourcePath() => ResourcePath;

        public override ILivingComponent[] GetSubLivingComponents()
        {
            return transform.Find("Attachments")
                .Children()
                .Select(subTransform => subTransform.GetComponent<ILivingComponent>())
                .Where(e => e != null)
                .ToArray();
        }

        public override MembraneGene GetGene() => gene ?? new MembraneGene();

        public override Transform OnInheritGene(MembraneGene inheritedGene)
        {
            base.OnInheritGene(inheritedGene);
            // GetComponent<CircleCollider2D>();
            Radius = inheritedGene.radius;
            return transform.Find("Attachments");
        }

        public void Attach(CircularAttachment attachment)
        {
            attachmentAdapter.AttachAt(attachment);
        }
    }
}