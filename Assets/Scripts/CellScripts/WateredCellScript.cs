using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateredCellScript : AbstractCellScript
{
    protected override void Start()
    {
        base.Start();
        Debug.Log("Soy el hijo");
        Debug.Log(timeToNextState);
        TriggerNextState();
    }

    protected override void TriggerNextState(){
        base.TriggerNextState();
        Object.Destroy(this.transform.GetChild(2).gameObject);
    }
}
