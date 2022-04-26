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
        private Rigidbody2D Rb { get; set; }


        protected override void Start()
        {
            proximityLayerMask = SimParams.Singleton.cellLayerMask |
                                 SimParams.Singleton.inertObstacleLayerMask;
            collidersInRange = new Collider2D[20];

            cell = GetComponentInParent<Cell.Cell>();
            Rb = GetComponentInParent<Rigidbody2D>();
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
            if (MicroHunters.IsHomeBase(cell)) ReactLikeHomeBase();
            else ReactLikeHunter();
        }

        private void ReactLikeHomeBase()
        {
            var cellTransform = cell.transform;
            actuatorLogits[SimParams.Singleton.birthCanalIndex][0] = 1f; // Birth
            // MicroHunters.ClassifyCell(cell);
            // var hunterCount=MicroHunters.TeamAHunters.Count();
            if (Time.frameCount > 200)
            {
                actuatorLogits[SimParams.Singleton.birthCanalIndex][0] = 0f; // Birth
            }
        }


        private void ReactLikeHunter()
        {
            // environment.CellCount; // Count of cells in the environment
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
            var classification = MicroHunters.ClassifyCell(otherCell);
            var targetPosition = new Vector3(0, 0, 0);

            // CellType type = MicroHunters.GetCellType(classification);
            // HunterTeam team = MicroHunters.GetHunterTeam(classification);
            // int TeamACount = MicroHunters.TeamAHunters.Count();
            // int TeamBCount = MicroHunters.TeamAHunters.Count();

            switch (classification)
            {
                case CellClassification.BaseA:
                    // Debug.Log("Home base A found");
                    break;
                case CellClassification.BaseB:
                    // Debug.Log("Home base B found");
                    break;
                case CellClassification.HunterA:
                    // Debug.Log("Hunter A found");
                    break;
                case CellClassification.HunterB:
                    // Debug.Log("Hunter B found");
                    break;
                case CellClassification.Sheep:
                    // Debug.Log("Sheep found");
                    break;
            }

            if (MicroHunters.ClassifyCell(cell) != classification)
            {
                targetPosition = otherCell.transform.position;
            }

            //Vector3 mousePosition = Input.mousePosition;
            Debug.DrawLine(cell.transform.position, targetPosition);

            var arriveAcceleration = Arrive(targetPosition, cellPos, Rb.velocity);

            var direction = targetPosition - cellTransform.transform.position;

            var flagellaForce = Vector2.Dot(arriveAcceleration, direction);

            var testOrientation = Mathf.Atan2(direction.y, direction.x);

            var targetOrientation = testOrientation * Mathf.Rad2Deg;

            // float dot = Vector3.Dot(direction, cellTransform.forward);
            // float target_orientation = Mathf.Acos( dot ) * Mathf.Rad2Deg;  

            var flagellaTorque = Align(targetOrientation * Mathf.Deg2Rad,
                (cellTransform.rotation.eulerAngles.z - 270) * Mathf.Deg2Rad, Rb.angularVelocity * Mathf.Deg2Rad);

            actuatorLogits[SimParams.Singleton.flagellaIndex][0] = (float)0.015 * flagellaForce; // Force
            actuatorLogits[SimParams.Singleton.flagellaIndex][1] = (float)0.05 * flagellaTorque; // Torque
            actuatorLogits[SimParams.Singleton.orificeIndex][0] = 1f; // Eat
        }


        private float DistanceToCollider(Collider2D otherCollider)
        {
            var closestPoint = otherCollider.ClosestPoint(cellPos);
            var distance = (closestPoint - cellPos).magnitude;
            return distance == 0 ? float.MaxValue : distance;
        }

        private static Vector2 Arrive(Vector2 targetPosition, Vector2 characterPosition, Vector2 characterVelocity)
        {
            var maxAcceleration = SimParams.Singleton.maxAcceleration;
            var maxSpeed = SimParams.Singleton.maxSpeed;
            var radiusDecel = SimParams.Singleton.arriveRadiusDecel;
            var radiusSat = SimParams.Singleton.arriveRadiusSat;
            var timeToTarget = SimParams.Singleton.arriveTimeToTarget;

            double targetSpeed = 0;

            var direction = targetPosition - characterPosition;
            var distance = Mathf.Sqrt(direction.x * direction.x + direction.y + direction.y);

            //Check if we are there, return no steering
            if (distance < radiusSat)
            {
                targetSpeed = 0;
            }

            //If we are outside the slowRadius, then move at max speed
            else if (distance > radiusDecel)
            {
                targetSpeed = maxSpeed;
            }
            else
            {
                targetSpeed = maxSpeed * distance / radiusDecel;
            }

            //The target velocity combines speed and direction
            var targetVelocity = direction;
            targetVelocity.Normalize();
            targetVelocity.x *= (float)targetSpeed;
            targetVelocity.y *= (float)targetSpeed;


            //Acceleration tries to get to target velocity

            var result = targetVelocity - characterVelocity;
            result.x /= (float)timeToTarget;
            result.y /= (float)timeToTarget;

            //Check if acceleration is too fast
            if (result.magnitude > maxAcceleration)
            {
                result = result.normalized * maxAcceleration;
            }

            return result;
        }

        private static float Align(float targetOrientation, float characterOrientation, float characterRotation)
        {
            var maxAngularAcceleration = SimParams.Singleton.maxAngularAcceleration;
            var maxRotation = SimParams.Singleton.maxRotation;
            var radiusDecel = SimParams.Singleton.alignRadiusDecel;
            var radiusSat = SimParams.Singleton.alignRadiusSat;
            var timeToTarget = SimParams.Singleton.alignTimeToTarget;

            float targetRotation;


            var rotation = SpacialUtils.NormalizeOrientation(targetOrientation - characterOrientation);

            var rotationSize = Mathf.Abs(rotation);


            //Check if we are there, return no steering
            if (rotationSize < radiusSat)
            {
                targetRotation = 0;
            }

            //If we are outside the slowRadius, then use maximum rotation
            else if (rotationSize > radiusDecel)
            {
                targetRotation = maxRotation;
            }

            //Otherwise calculate a scaled rotation
            else
            {
                targetRotation = maxRotation * rotationSize / radiusDecel;
            }

            //The final target rotation combines speed (already in the variable) and direction
            targetRotation *= rotation / rotationSize;

            //Acceleration tries to get to the target rotation
            var result = targetRotation - characterRotation;
            result /= timeToTarget;

            //Check if the acceleration is too great
            var angularAcceleration = Mathf.Abs(result);
            if (angularAcceleration > maxAngularAcceleration)
                result *= maxAngularAcceleration / angularAcceleration;


            return result;
        }
    }
}