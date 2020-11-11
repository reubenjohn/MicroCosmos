using System.Linq;
using Organelles;
using UnityEngine;

namespace Brains
{
    public abstract class AbstractBrain : MonoBehaviour
    {
        private ISensor[] sensors;
        public float[][] sensorLogits { get; private set; }
        private IActuator[] actuators { get; set; }
        public float[][] actuatorLogits { get; private set; }

        public void Start()
        {
            actuators = GetComponentInParent<Cell.Cell>().GetComponentsInChildren<IActuator>();
            Debug.Log($"Found {actuators.Length} actuators");
            actuatorLogits = actuators.Select(actuator =>
            {
                var logits = actuator.Connect();
                Debug.Log($"Found actuator {actuator.GetType().Name} with {logits.Length} logits");
                return logits;
            }).ToArray();

            sensors = GetComponentInParent<Cell.Cell>().GetComponentsInChildren<ISensor>();
            Debug.Log($"Found {sensors.Length} sensors");
            sensorLogits = sensors.Select(sensor =>
            {
                var logits = sensor.Connect();
                Debug.Log($"Found sensor {sensor.GetType().Name} with {logits.Length} logits");
                return logits;
            }).ToArray();
        }


        public virtual void Update()
        {
            for (var i = 0; i < sensors.Length; i++) sensors[i].Sense(sensorLogits[i]);
            React(sensorLogits);
            for (var i = 0; i < actuators.Length; i++) actuators[i].Actuate(actuatorLogits[i]);
        }

        public abstract void React(float[][] sensoryLogits);
    }
}