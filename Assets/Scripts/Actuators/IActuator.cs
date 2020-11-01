namespace Actuators
{
    internal interface IActuator
    {
        float[] Connect();
        void Actuate(float[] logits);
    }
}