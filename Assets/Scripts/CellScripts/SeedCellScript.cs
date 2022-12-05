using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedCellScript : AbstractCellScript
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Debug.Log("Soy el hijo");
        Debug.Log(timeToNextState);
        TriggerNextState();
    }
}
