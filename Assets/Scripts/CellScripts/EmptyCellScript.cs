using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
