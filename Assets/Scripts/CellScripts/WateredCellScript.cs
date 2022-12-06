using UnityEngine;

public class WateredCellScript : AbstractCellScript
{
    protected override void Suscribe()
    {
        cellId = manager.AddCell(this, wateredCell);
    }

    protected override void Unsuscribe()
    {
        manager.RemoveCell(cellId, wateredCell);
    }
}
