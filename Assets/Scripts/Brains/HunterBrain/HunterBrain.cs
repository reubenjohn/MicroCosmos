using System;
using System.Linq;
using Environment;
using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Util;

namespace Brains.HunterBrain
{
    public class HunterBrain : AbstractBrain, ILivingComponent<HunterBrainGene>
    {
        public const string ResourcePath = "Organelles/HunterBrain";

        public LayerMask cellLayerMask = SimParams.Singleton.cellLayerMask;
        public LayerMask chemicalBlobLayerMask = SimParams.Singleton.chemicalBlobLayerMask;
        public LayerMask inertObstacleLayerMask = SimParams.Singleton.inertObstacleLayerMask;

        private GenealogyGraphManager graphManager;
        private HunterBrainGene gene;
        private Cell.Cell cell;
        private readonly Guid hunterAGuid = Guid.Parse(SimParams.Singleton.hunterBaseAGuid);
        private readonly Guid hunterBGuid = Guid.Parse(SimParams.Singleton.hunterBaseBGuid);

        protected override void Start()
        {
            cell = GetComponentInParent<Cell.Cell>();
            graphManager = GetComponentInParent<GenealogyGraphManager>();
            base.Start();
            ResetBrain();
        }

        public string GetNodeName() => gameObject.name;

        Transform ILivingComponent.OnInheritGene(object inheritedGene) => OnInheritGene((HunterBrainGene)inheritedGene);

        public GeneTranscriber<HunterBrainGene> GetGeneTranscriber() => HunterBrainGeneTranscriber.Singleton;

        public HunterBrainGene GetGene() => gene;

        public Transform OnInheritGene(HunterBrainGene inheritedGene)
        {
            gene = inheritedGene;
            return transform;
        }

        IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();

        object ILivingComponent.GetGene() => GetGene();

        public string GetResourcePath() => ResourcePath;

        public JObject GetState() => new JObject();

        public void SetState(JObject state) { }

        public ILivingComponent[] GetSubLivingComponents() => new ILivingComponent[] { };

        private void ResetBrain()
        {
            var livingDescription = new HunterBrainDescription(
                sensorLogits.Select(logits => logits.Length).Sum(),
                actuatorLogits.Select(logits => logits.Length).Sum()
            );
            if (gene is IRepairableGene<HunterBrainGene, HunterBrainDescription> repairableGene)
                gene = repairableGene.RepairGene(livingDescription);
        }

        protected override void React()
        {
            try
            {
                if (IsHomeBase()) ReactLikeHomeBase();
                else ReactLikeHunter();
            }
            catch (InputSizeMismatchException)
            {
                ResetBrain();
                React();
            }
        }

        private void ReactLikeHomeBase()
        {
            actuatorLogits[SimParams.Singleton.birthCanalIndex][0] = 1f; // Birth
        }

        private Vector2 cellPos;

        private void ReactLikeHunter()
        {
            var cellTransform = cell.transform;
            cellPos = cellTransform.position;
            var collidersInRange = Physics2D.OverlapCircleAll(cellTransform.position,
                cellTransform.localScale.magnitude * SimParams.Singleton.cellVisibilityRangeRatio);

            if (collidersInRange.Length > 0)
            {
                var (closestCollider, closestDist) = ArrayUtils.ArgMin(collidersInRange, DistanceToCollider);
                var layerFlag = 1 << closestCollider.gameObject.layer;

                if ((layerFlag & SimParams.Singleton.cellLayerMask) != 0)
                {
                    var otherCell = closestCollider.GetComponentInParent<Cell.Cell>();
                    if (otherCell.GenealogyNode.Guid.ToString() == SimParams.Singleton.hunterBaseAGuid)
                    {
                        Debug.Log("Home base A found");
                    }
                    else if (otherCell.GenealogyNode.Guid.ToString() == SimParams.Singleton.hunterBaseBGuid)
                    {
                        Debug.Log("Home base B found");
                    }
                    else
                    {
                        var reproductionGuid =
                            graphManager.genealogyGraph.GetRelationsTo(otherCell.GenealogyNode.Guid)[0]
                                .From.Guid;
                        var asexualParentGuidString = graphManager.genealogyGraph.GetRelationsTo(reproductionGuid)[0]
                            .From.Guid.ToString();
                        if (asexualParentGuidString == SimParams.Singleton.hunterBaseAGuid)
                            Debug.Log("Hunter A found");
                        else if (asexualParentGuidString == SimParams.Singleton.hunterBaseBGuid)
                            Debug.Log("Hunter B found");
                        else
                            Debug.Log("Sheep found");
                    }

                    Debug.Log($"Nearby cell found: {otherCell.name}");
                }
            }

            actuatorLogits[SimParams.Singleton.flagellaIndex][0] = 1f; // Force
            actuatorLogits[SimParams.Singleton.flagellaIndex][1] = -0.03f; // Torque
            actuatorLogits[SimParams.Singleton.orificeIndex][0] = 1f; // Eat
        }


        private float DistanceToCollider(Collider2D otherCollider)
        {
            var closestPoint = otherCollider.ClosestPoint(cellPos);
            var distance = (closestPoint - cellPos).magnitude;
            return distance;
        }

        private bool IsHomeBase() => cell.GenealogyNode.Guid == hunterAGuid || cell.GenealogyNode.Guid == hunterBGuid;
    }
}