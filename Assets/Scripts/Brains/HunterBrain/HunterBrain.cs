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
        private Rigidbody2D rb { get; set; }
        private CellClassification cell_type;


        protected override void Start()
        {
            proximityLayerMask = SimParams.Singleton.cellLayerMask |
                                 SimParams.Singleton.inertObstacleLayerMask;
            collidersInRange = new Collider2D[20];

            cell = GetComponentInParent<Cell.Cell>();
            cell_type = MicroHunters.ClassifyCell(cell);
            rb = GetComponentInParent<Rigidbody2D>();
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
            if (MicroHunters.ClassifyCell(cell) == CellClassification.BaseA)
                MicroHunters.TeamABase = cell;
            else
                MicroHunters.TeamBBase = cell;
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
            Vector2 base_position;
            float angle_to_base = 0f;
            if (cell_type == CellClassification.HunterA)
            {
                base_position = MicroHunters.TeamABase.transform.position;
                angle_to_base = Mathf.Atan2(base_position.y - cellPos.y, base_position.x - cellPos.x);
            }
            else if (cell_type == CellClassification.HunterB)
            {
                base_position = MicroHunters.TeamBBase.transform.position;
                angle_to_base = Mathf.Atan2(base_position.y - cellPos.y, base_position.x - cellPos.x);
            }
            float wanderAngle = 2 * Mathf.PI * Mathf.PerlinNoise(cellPos.x, cellPos.y);
            if (angle_to_base != 0f)
                Debug.DrawLine(cellPos, cellPos + new Vector2(Mathf.Cos(angle_to_base), Mathf.Sin(angle_to_base)), Color.cyan, 1f);
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
            Vector2 closest_pos;
            float angle_away_from_closest = 0f;
            // CellType type = MicroHunters.GetCellType(classification);
            // HunterTeam team = MicroHunters.GetHunterTeam(classification);
            // int TeamACount = MicroHunters.TeamAHunters.Count();
            // int TeamBCount = MicroHunters.TeamAHunters.Count();

            if (classification == CellClassification.BaseA)
            {
                // Debug.Log("Home base A found");
            }
            else if (classification == CellClassification.BaseB)
            {
                // Debug.Log("Home base B found");
            }
            else if (classification == CellClassification.HunterA)
            {
                // Debug.Log("Hunter A found");
                if (cell_type == CellClassification.HunterA)
                {
                    closest_pos = otherCell.transform.position;
                    angle_away_from_closest = Mathf.Atan2(cellPos.y - closest_pos.y, cellPos.x - closest_pos.x);
                }
            }
            else if (classification == CellClassification.HunterB)
            {
                // Debug.Log("Hunter B found");
                if (cell_type == CellClassification.HunterB)
                {
                    closest_pos = otherCell.transform.position;
                    angle_away_from_closest = Mathf.Atan2(cellPos.y - closest_pos.y, cellPos.x - closest_pos.x);
                }
            }
            else if (classification == CellClassification.Sheep)
            {
                // Debug.Log("Sheep found");
            }
            
            if (angle_away_from_closest != 0f)
                Debug.DrawLine(cellPos, cellPos + new Vector2(Mathf.Cos(angle_away_from_closest), Mathf.Sin(angle_away_from_closest)), Color.white, 1f);

            //Vector3 mousePosition = Input.mousePosition;
            var targetPosition = new Vector3(5.9f, 0, 0);
            Debug.DrawLine(cell.transform.position, targetPosition);

            Vector2 arriveAcceleration = Arrive(targetPosition, cellPos, rb.velocity);

            Vector3 direction = targetPosition - cellTransform.transform.position;

            float flagellaForce = Vector2.Dot(arriveAcceleration, direction);

            var test_orientation = Mathf.Atan2(direction.y, direction.x);

            float target_orientation = test_orientation * Mathf.Rad2Deg;

            // float dot = Vector3.Dot(direction, cellTransform.forward);
            // float target_orientation = Mathf.Acos( dot ) * Mathf.Rad2Deg;  

            var flagellaTorque = Align(target_orientation * Mathf.Deg2Rad,
                (cellTransform.rotation.eulerAngles.z - 270) * Mathf.Deg2Rad, rb.angularVelocity * Mathf.Deg2Rad);

            actuatorLogits[SimParams.Singleton.flagellaIndex][0] = (float)0.005 * flagellaForce; // Force
            actuatorLogits[SimParams.Singleton.flagellaIndex][1] = (float)0.05 * flagellaTorque; // Torque
            actuatorLogits[SimParams.Singleton.orificeIndex][0] = 1f; // Eat
        }


        private float DistanceToCollider(Collider2D otherCollider)
        {
            var closestPoint = otherCollider.ClosestPoint(cellPos);
            var distance = (closestPoint - cellPos).magnitude;
            return distance == 0 ? float.MaxValue : distance;
        }

        private Vector2 Arrive(Vector2 target_position, Vector2 character_position, Vector2 character_velocity)
        {
            double maxAcceleration = SimParams.Singleton.maxAcceleration;
            double maxSpeed = SimParams.Singleton.maxSpeed;
            double radiusDecel = SimParams.Singleton.arriveRadiusDecel;
            double radiusSat = SimParams.Singleton.arriveRadiusSat;
            double timeToTarget = SimParams.Singleton.arriveTimeToTarget;

            double targetSpeed = 0;
            Vector2 targetVelocity;

            Vector2 result;
            Vector2 direction = target_position - character_position;
            float distance = Mathf.Sqrt(direction.x * direction.x + direction.y + direction.y);

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
            targetVelocity = direction;
            targetVelocity.Normalize();
            targetVelocity.x *= (float)targetSpeed;
            targetVelocity.y *= (float)targetSpeed;


            //Acceleration tries to get to target velocity

            result = targetVelocity - character_velocity;
            result.x /= (float)timeToTarget;
            result.y /= (float)timeToTarget;

            //Check if acceleration is too fast
            if (result.magnitude > maxAcceleration)
            {
                result.Normalize();
                result.x *= (float)maxAcceleration;
                result.y *= (float)maxAcceleration;
            }

            return result;
        }

        private float Align(float target_orientation, float character_orientation, float character_rotation)
        {
            double maxAngularAcceleration = SimParams.Singleton.maxAngularAcceleration;
            double maxRotation = SimParams.Singleton.maxRotation;
            double radiusDecel = SimParams.Singleton.alignRadiusDecel;
            double radiusSat = SimParams.Singleton.alignRadiusSat;
            double timeToTarget = SimParams.Singleton.alignTimeToTarget;

            double targetRotation = 0;


            float result = 0;
            double rotation = target_orientation - character_orientation;
            rotation = rotation % 6.28;
            if (Mathf.Abs((float)rotation) > 6.28)
                rotation = (rotation % 6.28);
            if (rotation > 3.14)
                rotation -= 3.14;
            if (rotation < -3.14)
                rotation += 3.14;

            double rotationSize = Mathf.Abs((float)rotation);


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
            result = (float)targetRotation - character_rotation;
            result /= (float)timeToTarget;

            //Check if the acceleration is too great
            float angularAcceleration = Mathf.Abs(result);
            if (angularAcceleration > maxAngularAcceleration)
            {
                result /= angularAcceleration;
                result *= (float)maxAngularAcceleration;
            }

            return result;
        }
    }
}