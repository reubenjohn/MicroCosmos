using System;
using UnityEngine;

namespace Environment
{
    public class SimParams : MonoBehaviour
    {
        public string hunterBaseAGuidString = "7f146de4-9367-4bf8-bef0-82b08b5c10c6";
        public string hunterBaseBGuidString = "7f146de4-9367-4bf8-bef0-82b08b5c10c7";

        public Guid hunterBaseAGuid => Guid.Parse(hunterBaseAGuidString);
        public Guid hunterBaseBGuid => Guid.Parse(hunterBaseBGuidString);

        public int flagellaIndex = 1;
        public int birthCanalIndex = 2;
        public int orificeIndex = 3;
        public int proximitySensorIndex = 0;
        public int proximitySensorWallIndex = 2;
        public float hunterVisibilityRangeRatio = 5f;
        public float sheepVisibilityRangeRatio = 3f;


        //Params for Arrive
        public float maxAcceleration = 5;
        public float maxSpeed = 5;
        public float arriveRadiusDecel = 1;
        public float arriveRadiusSat = 0.5f;
        public float arriveTimeToTarget = 0.25f;


        //Params for Align
        public float maxRotation = 10;
        public float maxAngularAcceleration = 1;
        public float alignRadiusDecel = 1;
        public float alignRadiusSat = 0.1f;
        public float alignTimeToTarget = 0.25f;

        //Params for Wander
        public float wanderFluctuation1 = 0.1f;
        public float wanderFluctuation2 = 0.4f;
        public float wanderRangeFluctuation = 0.4f;
        public float wanderSweep = 6.28f;
        public float wanderRange = 0.5f;

        public LayerMask cellLayerMask;
        public LayerMask chemicalBlobLayerMask;
        public LayerMask inertObstacleLayerMask;

        public static SimParams Singleton { get; private set; }

        private void Start()
        {
            Singleton = GameObject.Find("Environment").GetComponent<SimParams>();
        }
    }
}