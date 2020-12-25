using UnityEngine;

[RequireComponent(typeof(Light))]
[ExecuteInEditMode]
public class RelativeLightBehavior : MonoBehaviour
{
    public float range;
    public float intensity;

    private Light relativeLight;

    private void Start()
    {
        relativeLight = GetComponent<Light>();
    }


    private void Update()
    {
        var localScaleMagnitude = transform.localScale.magnitude;
        relativeLight.range = range * localScaleMagnitude;
        relativeLight.intensity = intensity * localScaleMagnitude;
    }
}