using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedCellScript : AbstractCellScript
{

    protected override void Suscribe()
    {
        cellId = manager.AddCell(this, seedCell);
    }

    protected override void Unsuscribe()
    {
        manager.RemoveCell(cellId, seedCell);
    }
}
