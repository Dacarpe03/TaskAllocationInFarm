using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyCellScript : AbstractCellScript
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Debug.Log("Soy el hijo");
        Debug.Log(timeToNextState);
        TriggerNextState();
    }

    protected override void Suscribe()
    {
        cellId = manager.AddCell(this, emptyCell);
    }

    protected override void Unsuscribe()
    {
        throw new System.NotImplementedException();
    }
}
