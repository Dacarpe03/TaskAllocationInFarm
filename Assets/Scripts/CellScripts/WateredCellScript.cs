using System.Collections;
using UnityEngine;

public class WateredCellScript : AbstractCellScript
{
    protected override IEnumerator Countdown()
    {
        Destroy(this.transform.GetChild(2).gameObject);
        yield return new WaitForSeconds(timeToNextState);
        GameObject newState = Instantiate(nextCellStateObject, this.transform.position, Quaternion.identity);
        newState.GetComponent<AbstractCellScript>().SetScale(this.transform.localScale);
        Unsuscribe();
        Destroy(this.gameObject);
    }
    protected override void Suscribe()
    {
        cellId = manager.AddCell(this, wateredCell);
    }

    protected override void Unsuscribe()
    {
        manager.RemoveCell(cellId, wateredCell);
    }
}
