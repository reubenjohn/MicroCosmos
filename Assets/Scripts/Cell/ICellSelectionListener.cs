namespace Cell
{
    public interface ICellSelectionListener
    {
        void OnCellSelectionChange(Cell cell, bool select);
    }
}