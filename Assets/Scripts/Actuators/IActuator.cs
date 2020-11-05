namespace Actuators
{
    public interface IActuator
    {
        float[] Connect();
        void Actuate(float[] logits);
    }
}