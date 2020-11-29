namespace Cell
{
    public interface ICellColonyListener
    {
        void OnSave(string saveDirectory);
        void OnLoad(string persistenceFilePath);
    }
}