﻿using System;
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
        public double maxAcceleration = 5;
        public double maxSpeed = 5;
        public double arriveRadiusDecel = 1;
        public double arriveRadiusSat = 0.5;
        public double arriveTimeToTarget = 0.25;
        
        
        //Params for Align
        public double maxRotation = 10;
        public double maxAngularAcceleration = 1;
        public double alignRadiusDecel = 1;
        public double alignRadiusSat = 0.5;
        public double alignTimeToTarget = 0.25;

        
        
        
        

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