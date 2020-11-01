using UnityEngine;

namespace Actuators
{
    public class OrificeActuator : MonoBehaviour, IActuator
    {
        public void Actuate(float[] logits)
        {
        }

        public float[] Connect()
        {
            return new float[0];
        }
    }
}