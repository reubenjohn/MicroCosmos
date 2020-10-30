using System;
using Newtonsoft.Json;

[Serializable]
public class FlagellaGene : IGene<FlagellaGene>
{
    public float linearPower = 250f;
    public float angularPower = 10f;

    public FlagellaGene(float linearPower, float angularPower)
    {
        this.linearPower = linearPower;
        this.angularPower = angularPower;
    }

    public override string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }

    public override FlagellaGene Deserialize(string sequence)
    {
        return JsonConvert.DeserializeObject<FlagellaGene>(sequence);
    }

    public override FlagellaGene Mutate()
    {
        return new FlagellaGene(
            linearPower.MutateClamped(10f, .1f, float.MaxValue),
            angularPower.MutateClamped(10f, .1f, float.MaxValue)
        );
    }
}
