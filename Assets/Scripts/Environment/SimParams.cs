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
        public float hunterVisibilityRangeRatio = 5f;
        public float sheepVisibilityRangeRatio = 3f;

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