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
        public const string ResourcePath = "Organelles/Membrane1";
        private const float MinMembraneRatio = .05f;
        private readonly CircularAttachmentRing attachmentAdapter = new CircularAttachmentRing();
        private CellCauldron.CellCauldron cauldron;
        private Cell.Cell cell;
        private SpriteMask ringInnerMask;

        private float Radius
        {
            get => transform.localScale.x * .5f;
            set => transform.localScale = Vector3.one * value * 2f;
        }

        private float RelativeInnerRadius
        {
            get
            {
                var totalMass = cauldron.TotalMass;
                return (totalMass - cauldron[Substance.Skin]) / totalMass;
            }
        }

        private void Start()
        {
            cell = GetComponentInParent<Cell.Cell>();
            cauldron = GetComponentInParent<CellCauldron.CellCauldron>();
            ringInnerMask = transform.Find("Ring").GetComponentInChildren<SpriteMask>();
        }

        private void Update()
        {
            var relativeInnerRadius = RelativeInnerRadius;
            ringInnerMask.transform.localScale = Vector3.one * relativeInnerRadius;
            var ratio = ThicknessRatio(relativeInnerRadius);
            var belowTarget = ratio < gene.relativeThickness;
            if (belowTarget && cauldron[Substance.Fat] > 0)
            {
                cauldron.Convert(RecipeBook.Singleton[Recipe.GrowSkin]);
                ratio = ThicknessRatio(RelativeInnerRadius);
            }

            if (cell.IsInFocus) Grapher.Log(ratio, "Membrane.ThicknessRatio");
            if (ratio < MinMembraneRatio || cauldron.TotalMass < Cell.Cell.MinMass)
                cell.Die();
        }

        private float ThicknessRatio(float relativeInnerRadius) => 1 - relativeInnerRadius;

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
            Radius = inheritedGene.radius;
            return transform.Find("Attachments");
        }

        public void Attach(CircularAttachment attachment) => attachmentAdapter.AttachAt(attachment);
    }
}