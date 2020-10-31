using System;
using Newtonsoft.Json;

[Serializable]
public class FlagellaGene
{
    public float linearPower = 250f;
    public float angularPower = 10f;

    public FlagellaGene(float linearPower, float angularPower)
    {
        this.linearPower = linearPower;
        this.angularPower = angularPower;
    }
}
