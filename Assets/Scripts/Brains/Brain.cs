using System.Linq;
using UnityEngine;

public class Brain : MonoBehaviour
{
    private IActuator[] actuators { get; set; }
    public float[][] actuatorLogits { get; private set; }

    // Start is called before the first frame update
    public void Start()
    {
        actuators = GetComponentsInChildren<IActuator>();
        Debug.Log($"Found {actuators.Length} actuators");
        actuatorLogits = actuators.Select(actuator =>
        {
            var logits = actuator.Connect();
            Debug.Log($"Found actuator {actuator.GetType().Name} with {logits.Length} logits");
            return logits;
        }).ToArray();
    }


    public virtual void Update()
    {
        for (var i = 0; i < actuators.Length; i++) actuators[i].Actuate(actuatorLogits[i]);
    }
}