public class EmptyCellScript : AbstractCellScript
{
    protected override void Suscribe()
    {
        cellId = manager.AddCell(this, emptyCell);
    }

    protected override void Unsuscribe()
    {
        manager.RemoveCell(cellId, emptyCell);
    }
}
