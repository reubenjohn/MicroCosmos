﻿using System.Linq;
using Organelles;
using UnityEngine;

namespace Brains
{
    public abstract class AbstractBrain : MonoBehaviour
    {
        private string[] actuatorLogitLabels;
        private Cell.Cell cell;
        private ISensor[] sensors;
        protected float[][] sensorLogits { get; private set; }
        protected IActuator[] actuators { get; private set; }
        protected float[][] actuatorLogits { get; private set; }

        protected virtual void Start()
        {
            cell = GetComponentInParent<Cell.Cell>();
            actuators = cell.GetComponentsInChildren<IActuator>();
            actuatorLogits = actuators.Select(actuator =>
            {
                var logits = actuator.Connect();
                // Debug.Log($"Found actuator {actuator.GetType().Name} with {logits.Length} logits");
                return logits;
            }).ToArray();

            sensors = cell.GetComponentsInChildren<ISensor>();
            sensorLogits = sensors.Select(sensor =>
            {
                var logits = sensor.Connect();
                // Debug.Log($"Found sensor {sensor.GetType().Name} with {logits.Length} logits");
                return logits;
            }).ToArray();

            actuatorLogitLabels = actuators.SelectMany((actuator, actuatorI) =>
            {
                var actuatorType = actuator.GetActuatorType();
                return actuatorLogits[actuatorI].Select((_, i) => $"{actuatorType.Split('.').Last()}[{i}]");
            }).ToArray();
        }


        public virtual void Update()
        {
            for (var i = 0; i < sensors.Length; i++) sensors[i].Sense(sensorLogits[i]);
            React();
            for (var i = 0; i < actuators.Length; i++) actuators[i].Actuate(actuatorLogits[i]);

            if (cell.IsInFocus)
            {
                var labelI = 0;
                foreach (var logits in actuatorLogits)
                foreach (var logit in logits)
                    if (!actuatorLogitLabels[labelI++].Contains("Cauldron"))
                        Grapher.Log(logit, actuatorLogitLabels[labelI - 1]);
            }
        }

        protected abstract void React();
    }
}