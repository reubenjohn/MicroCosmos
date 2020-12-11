using Environment;
using Genealogy.Graph;
using Genetics;
using Persistence;
using Structural;
using UnityEngine;

namespace Organelles.BirthCanal
{
    public class BirthCanal : AbstractLivingComponent<BirthCanalGene>, IActuator
    {
        public Control.BinaryControlVariable birthSignal = new Control.BinaryControlVariable(1);
        private CircularAttachment attachment;
        private Cell.Cell cell;

        private void Start()
        {
            cell = GetComponentInParent<Cell.Cell>();
        }

        public float[] Connect()
        {
            return new float[1];
        }

        public void Actuate(float[] logits)
        {
            var giveBirthSignal = birthSignal.FeedInput(logits[0] > -.5f, logits[0] < .5, Time.deltaTime);
            if (Mathf.Approximately(giveBirthSignal, 1))
                GiveBirth();
            if (cell.IsInFocus)
            {
                Grapher.Log(logits[0], "GiveBirth?", Color.magenta);
                Grapher.Log(giveBirthSignal, "GiveBirth!", Color.red);
            }
        }

        private void GiveBirth()
        {
            birthSignal.Value = 0;

            var parent = cell;
            var geneTree = GeneNode.GetMutated(parent);

            var t = transform;
            var cellColony = GetComponentInParent<CellColony>();
            var child = GeneNode.Load(geneTree, cellColony.transform, t.Find("SpawnPoint").position, t.rotation);

            var childCell = child.GetComponent<Cell.Cell>();
            if (parent.GenealogyGraphManager)
                childCell.GenealogyNode = parent.GenealogyGraphManager.RegisterAsexualCellBirth(
                    new Node[] {parent.GenealogyNode}, childCell);
        }

        public override GeneTranscriber<BirthCanalGene> GetGeneTranscriber()
        {
            return BirthCanalGeneTranscriber.Singleton;
        }

        public override BirthCanalGene GetGene()
        {
            return gene ?? new BirthCanalGene();
        }

        public override Transform OnInheritGene(BirthCanalGene inheritedGene)
        {
            attachment = new CircularAttachment(
                transform,
                inheritedGene.circularMembranePreferredAttachmentAngle,
                inheritedGene.circularMembraneAngularDisplacement
            );
            var membrane = GetComponentInParent<Membrane.Membrane>();
            membrane.Attach(attachment);
            return base.OnInheritGene(inheritedGene);
        }

        public override string GetResourcePath()
        {
            return "Organelles/BirthCanal1";
        }
    }
}