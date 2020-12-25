using UnityEngine;

namespace Organelles
{
    public class OrificeActuator : MonoBehaviour, IActuator
    {
        private static readonly string ActuatorType = typeof(OrificeActuator).FullName;

        public void Actuate(float[] logits) { }

        public string GetActuatorType() => ActuatorType;

        public float[] Connect() => new float[0];
    }
}