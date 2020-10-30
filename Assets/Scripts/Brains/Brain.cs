using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Brain : MonoBehaviour
{
    internal IActuator[] actuators { get; private set; }
    public float[][] actuatorLogits { get; private set; }

    // Start is called before the first frame update
    public void Start()
    {
        actuators = GetComponentsInChildren<IActuator>();
        Debug.Log(string.Format("Found {0} actuators", actuators.Length));
        actuatorLogits = actuators.Select(actuator =>
        {
            float[] logits = actuator.Connect();
            Debug.Log(string.Format("Found actuator {0} with {1} logits", actuator.GetType().Name, logits.Length));
            return logits;
        }).ToArray();
    }

    // Update is called once per frame
    public void Update()
    {
        for (int i = 0; i < actuators.Length; i++)
        {
            actuators[i].Actuate(actuatorLogits[i]);
        }
    }
}
