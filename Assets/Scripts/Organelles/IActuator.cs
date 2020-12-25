namespace Organelles
{
    public interface IActuator
    {
        string GetActuatorType();
        float[] Connect();
        void Actuate(float[] logits);
    }
}