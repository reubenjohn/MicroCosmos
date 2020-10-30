internal interface IActuator
{
    float[] Connect();
    void Actuate(float[] logits);
}