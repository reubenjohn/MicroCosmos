using Cell;
using Chemistry;
using ChemistryMicro;
using Environment;
using Genealogy.Graph;
using Genetics;
using Persistence;
using Structural;
using UnityEngine;
using Util;

namespace Organelles.BirthCanal
{
    public class BirthCanal : AbstractLivingComponent<BirthCanalGene>, IActuator
    {
        public static readonly string ResourcePath = "Organelles/BirthCanal1";
        public static readonly string ActuatorType = typeof(BirthCanal).FullName;

        private static readonly Mixture<Substance> UnitFatMix = new MixtureDictionary<Substance>
        {
            {Substance.Fat, 1f}
        }.ToMixture();

        public Control.BinaryControlVariable birthSignal = new Control.BinaryControlVariable(1);

        private Cell.Cell cell;

        private Vector3 SpawnPoint => transform.Find("SpawnPoint").position;

        private void Start()
        {
            cell = GetComponentInParent<Cell.Cell>();
        }

        public string GetActuatorType() => ActuatorType;

        public float[] Connect() => new float[1];

        public void Actuate(float[] logits)
        {
            var giveBirthSignal = birthSignal.FeedInput(logits[0] > 0, logits[0] < 0, Time.deltaTime);
            if (Mathf.Approximately(giveBirthSignal, 1))
                TryGiveBirth();
            if (cell.IsInFocus) Grapher.Log(giveBirthSignal, "GiveBirth!", Color.red);
        }

        private void TryGiveBirth()
        {
            birthSignal.Value = 0;

            var geneTree = GeneNode.GetMutated(cell);

            var cauldron = cell.Cauldron;
            var mamaFat = cauldron[Substance.Fat];
            var mamaMass = cauldron.TotalMass;
            var maxPossibleBabyMass = mamaMass * .4f;
            var minPossibleBabyMass = mamaMass * .1f;
            var maxCurrentBabyMass = mamaFat * .5f;
            if (mamaFat > minPossibleBabyMass)
            {
                var babyGene = (CellGene) geneTree.gene;
                var babyMix = new Mixture<Substance>(
                    EnumUtils.ParseNamedDictionary<Substance, float>(babyGene.cauldron.initialCauldron)) * mamaMass;
                var babyMass = babyMix.TotalMass;

                if (babyMass > maxPossibleBabyMass || babyMass < minPossibleBabyMass || babyMass < Cell.Cell.MinMass)
                {
                    DieInChildBirth();
                }
                else if (babyMass > maxCurrentBabyMass)
                {
                    birthSignal.inputSensitivity *= .99f;
                }
                else
                {
                    birthSignal.inputSensitivity += (1 - birthSignal.inputSensitivity) * .1f;
                    SpawnBaby(geneTree, babyMix.TotalMass);
                }
            }
        }

        private void SpawnBaby(GeneNode geneTree, float babyMass)
        {
            var cellColony = GetComponentInParent<CellColony>();
            var genealogyGraphManager = GetComponentInParent<GenealogyGraphManager>();
            var childGenealogyNode = genealogyGraphManager.RegisterAsexualCellBirth(new Node[] {cell.GenealogyNode});
            var cellState = Cell.Cell.GetState(
                childGenealogyNode.Guid,
                SpawnPoint, transform.rotation,
                CellCauldron.CellCauldron.GetState(UnitFatMix * babyMass, true));
            cellColony.SpawnCell(geneTree, cellState, cell.Cauldron);
        }

        private void DieInChildBirth()
        {
            cell.Die();
        }

        private void Miscarriage(CellCauldron.CellCauldron cauldron, Mixture<Substance> miscarriageMix)
        {
            GetComponentInParent<ChemicalSink>().Dump(SpawnPoint, cauldron, miscarriageMix);
        }

        public override GeneTranscriber<BirthCanalGene> GetGeneTranscriber() => BirthCanalGeneTranscriber.Singleton;

        public override BirthCanalGene GetGene() => gene ?? new BirthCanalGene();

        public override Transform OnInheritGene(BirthCanalGene inheritedGene)
        {
            GetComponentInParent<Membrane.Membrane>()
                .Attach(new CircularAttachment(
                    transform,
                    inheritedGene.circularMembraneAttachment
                ));
            return base.OnInheritGene(inheritedGene);
        }

        public override string GetResourcePath() => ResourcePath;
    }
}