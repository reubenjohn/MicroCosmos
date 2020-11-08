namespace Organelles
{
    public interface ISensor
    {
        float[] Connect();
        void Sense(float[] logits);
    }
}