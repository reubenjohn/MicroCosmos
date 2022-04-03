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

        private LayerMask proximityLayerMask;

        private GenealogyGraphManager graphManager;
        private HunterBrainGene gene;
        private Cell.Cell cell;
        private Vector2 cellPos;
        private Collider2D[] collidersInRange;

        protected override void Start()
        {
            proximityLayerMask = SimParams.Singleton.cellLayerMask |
                                 SimParams.Singleton.inertObstacleLayerMask;
            collidersInRange = new Collider2D[20];

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
            if (IsHomeBase()) ReactLikeHomeBase();
            else ReactLikeHunter();
        }

        private void ReactLikeHomeBase()
        {
            actuatorLogits[SimParams.Singleton.birthCanalIndex][0] = 1f; // Birth
        }


        private void ReactLikeHunter()
        {
            var cellTransform = cell.transform;
            cellPos = cellTransform.position;
            var nDetected = Physics2D.OverlapCircleNonAlloc(cellPos,
                cellTransform.localScale.magnitude * SimParams.Singleton.hunterVisibilityRangeRatio, collidersInRange,
                proximityLayerMask.value);

            if (nDetected <= 0)
                return;

            var (closestCollider, dist) = ArrayUtils.ArgMin(collidersInRange.Take(nDetected), DistanceToCollider);
            if (closestCollider == null)
                return;

            var layerFlag = 1 << closestCollider.gameObject.layer;

            if ((layerFlag & SimParams.Singleton.cellLayerMask) == 0)
                return;

            var otherCell = closestCollider.GetComponentInParent<Cell.Cell>();
            if (otherCell.GenealogyNode.Guid == SimParams.Singleton.hunterBaseAGuid)
            {
                // Debug.Log("Home base A found");
            }
            else if (otherCell.GenealogyNode.Guid == SimParams.Singleton.hunterBaseBGuid)
            {
                // Debug.Log("Home base B found");
            }
            else
            {
                var reproductionGuid =
                    graphManager.genealogyGraph.GetRelationsTo(otherCell.GenealogyNode.Guid)[0]
                        .From.Guid;
                var asexualParentGuid = graphManager.genealogyGraph.GetRelationsTo(reproductionGuid)[0]
                    .From.Guid;
                if (asexualParentGuid == SimParams.Singleton.hunterBaseAGuid)
                {
                    // Debug.Log("Hunter A found");
                }
                else if (asexualParentGuid == SimParams.Singleton.hunterBaseBGuid)
                {
                    // Debug.Log("Hunter B found");
                }
            }

            var closestObject_x = closestCollider.transform.position.x;
            var closestObject_y = closestCollider.transform.position.y;

            var current_x = cellPos.x;
            var current_y = cellPos.y;

            var current_rotation = cellTransform.rotation.z;

            actuatorLogits[SimParams.Singleton.flagellaIndex][0] = 1f; // Force
            actuatorLogits[SimParams.Singleton.flagellaIndex][1] = -0.03f; // Torque
            actuatorLogits[SimParams.Singleton.orificeIndex][0] = 1f; // Eat
        }


        private float DistanceToCollider(Collider2D otherCollider)
        {
            var closestPoint = otherCollider.ClosestPoint(cellPos);
            var distance = (closestPoint - cellPos).magnitude;
            return distance == 0 ? float.MaxValue : distance;
        }

        private bool IsHomeBase() =>
            cell.GenealogyNode.Guid == SimParams.Singleton.hunterBaseAGuid ||
            cell.GenealogyNode.Guid == SimParams.Singleton.hunterBaseBGuid;
    }
}