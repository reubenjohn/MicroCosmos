using UnityEngine;

[RequireComponent(typeof(Light))]
[ExecuteInEditMode]
public class RelativeLightRangeBehavior : MonoBehaviour
{
    public float range;

    private Light relativeLight;

    private void Start() => relativeLight = GetComponent<Light>();


    private void Update() => relativeLight.range = range * transform.lossyScale.magnitude;
}