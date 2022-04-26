using System;
using System.Linq;
using Environment;
using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

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
        public CellClassification CellClassification { get; private set; }
        private HunterBrain friendlyBase;
        private HunterBrain enemyBase;
        private HunterTeam team;

        private float wanderSeed;

        protected override void Start()
        {
            proximityLayerMask = SimParams.Singleton.cellLayerMask |
                                 SimParams.Singleton.inertObstacleLayerMask;
            collidersInRange = new Collider2D[20];

            cell = GetComponentInParent<Cell.Cell>();
            Rb = GetComponentInParent<Rigidbody2D>();
            CellClassification = MicroHunters.ClassifyCell(cell);
            team = MicroHunters.GetHunterTeam(CellClassification);

            wanderSeed = Random.Range(-100, 100);

            if (MicroHunters.GetCellType(CellClassification) == CellType.Base)
            {
                if (team == HunterTeam.TeamA)
                    MicroHunters.TeamABase = cell;
                else
                    MicroHunters.TeamBBase = cell;
            }
            else if (MicroHunters.GetCellType(CellClassification) == CellType.Hunter)
            {
                friendlyBase = MicroHunters.GetHunterTeam(CellClassification) == HunterTeam.TeamA
                    ? MicroHunters.TeamABase.GetComponent<HunterBrain>()
                    : MicroHunters.TeamBBase.GetComponent<HunterBrain>();
                enemyBase =
                    MicroHunters.GetHunterTeam(CellClassification) ==
                    HunterTeam.TeamA // FIXME once the enemy base also spawns
                        ? MicroHunters.TeamABase.GetComponent<HunterBrain>()
                        : MicroHunters.TeamBBase.GetComponent<HunterBrain>();
            }

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
            actuatorLogits[SimParams.Singleton.birthCanalIndex][0] = 1f; // Birth
            // MicroHunters.ClassifyCell(cell);
            // var hunterCount=MicroHunters.TeamAHunters.Count();
            if (Time.frameCount > 200)
            {
                actuatorLogits[SimParams.Singleton.birthCanalIndex][0] = 0f; // Birth
            }
        }

        private Vector2 GetTargetPosition()
        {
            // environment.CellCount; // Count of cells in the environment
            var cellTransform = cell.transform;

            var nDetected = Physics2D.OverlapCircleNonAlloc(cellPos,
                cellTransform.localScale.magnitude * SimParams.Singleton.hunterVisibilityRangeRatio, collidersInRange,
                proximityLayerMask.value);

            if (nDetected <= 0)
                return WanderTarget();

            var (closestCollider, dist) = ArrayUtils.ArgMin(collidersInRange.Take(nDetected), DistanceToCollider);
            if (closestCollider == null)
                return WanderTarget();

            var layerFlag = 1 << closestCollider.gameObject.layer;

            if ((layerFlag & SimParams.Singleton.cellLayerMask) == 0)
                return WanderTarget();

            var otherCell = closestCollider.GetComponentInParent<Cell.Cell>();
            Vector2 otherPos = otherCell.transform.position;
            var otherCellClassification = MicroHunters.ClassifyCell(otherCell);

            if (otherCellClassification == CellClassification.Sheep ||
                MicroHunters.GetHunterTeam(otherCellClassification) != team)
            {
                // Debug.Log("Sheep or enemy base/hunter found");
                return otherPos;
            }

            return WanderTarget();
        }

        private Vector2 WanderTarget()
        {
            var probe1 = wanderSeed + SimParams.Singleton.wanderFluctuation1 * Time.time;
            var probe2 = 100f + wanderSeed + SimParams.Singleton.wanderFluctuation2 * Time.time;
            var wanderRotation = Mathf.PerlinNoise(probe1, probe1)
                                 + Mathf.PerlinNoise(probe2, probe2)
                                 - 0.5f
                                 - 0.5f;
            var wanderAngle = transform.eulerAngles.z * Mathf.PI / 180f +
                              SimParams.Singleton.wanderSweep * wanderRotation;
            // Debug.Log(Mathf.PerlinNoise(Time.time, cellPos.magnitude) -
            //           Mathf.PerlinNoise(Time.time + 0.1f, cellPos.magnitude));
            var rangeProbe = -100f + wanderSeed + SimParams.Singleton.wanderRangeFluctuation * Time.time;
            var targetPosition = cellPos + new Vector2(Mathf.Sin(wanderAngle), Mathf.Cos(wanderAngle)) *
                (0.5f + Mathf.PerlinNoise(rangeProbe, rangeProbe)) * SimParams.Singleton.wanderRange;
            return targetPosition;
        }

        private void ReactLikeHunter()
        {
            var cellTransform = cell.transform;
            cellPos = cellTransform.position;
            var targetPosition = GetTargetPosition();
            // if (angleAwayFromClosest != 0f)
            //     Debug.DrawLine(cellPos,
            //         cellPos + new Vector2(Mathf.Cos(angleAwayFromClosest), Mathf.Sin(angleAwayFromClosest)),
            //         Color.white, 1f);

            //Vector3 mousePosition = Input.mousePosition;
            Debug.DrawLine(cellPos, targetPosition);

            var arriveAcceleration = Arrive(targetPosition, cellPos, Rb.velocity);

            var direction = targetPosition - cellPos;

            var flagellaForce = arriveAcceleration.magnitude * Mathf.Max(0f,
                Mathf.Cos(Vector2.Angle(arriveAcceleration, direction) * 180 / Mathf.PI));

            var testOrientation = Mathf.Atan2(direction.y, direction.x);

            var targetOrientation = testOrientation * Mathf.Rad2Deg;

            // float dot = Vector3.Dot(direction, cellTransform.forward);
            // float target_orientation = Mathf.Acos( dot ) * Mathf.Rad2Deg;  

            var flagellaTorque = Align(targetOrientation * Mathf.Deg2Rad,
                (cellTransform.rotation.eulerAngles.z - 270) * Mathf.Deg2Rad, Rb.angularVelocity * Mathf.Deg2Rad);

            actuatorLogits[SimParams.Singleton.flagellaIndex][0] = 2f * flagellaForce; // Force
            actuatorLogits[SimParams.Singleton.flagellaIndex][1] = 0.05f * flagellaTorque; // Torque
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
            var distance = Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y);

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
            targetRotation *= Mathf.Sign(rotation);

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