using UnityEngine;

namespace Organelles.Orifice
{
    public class Orifice : MonoBehaviour, IActuator
    {
        private static readonly string ActuatorType = typeof(Orifice).FullName;

        public void Actuate(float[] logits) { }

        public string GetActuatorType() => ActuatorType;

        public float[] Connect() => new float[0];
    }
}