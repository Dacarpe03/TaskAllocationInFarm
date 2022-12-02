using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCellScript : MonoBehaviour
{
    [Header("NextObject")]
    [SerializeField] protected GameObject nextCellStateObject;

    [Header("AnimationObject")]
    [SerializeField] protected GameObject animationObject;

    [Header("Timers")]
    [SerializeField] protected float timeToNextState = 2f;

    protected CellSpawnerScript a;

    protected virtual void Start(){
        a = FindObjectOfType<CellSpawnerScript>();
        Debug.Log("Lo tengo");

    }

    protected void CreateTask(){
        Debug.Log("Creating task");
    }

    protected void TriggerNextState(){
        StartAnimation();
        StartCoroutine(Countdown());
    }

    protected void StartAnimation(){
        //Instantiate(animationObject, this.transform.position, Quaternion.identity)
    }

    protected IEnumerator Countdown(){
        Debug.Log("Countdown");
        yield return new WaitForSeconds(timeToNextState);
        GameObject newState = Instantiate(nextCellStateObject, this.transform.position, Quaternion.identity);
        newState.GetComponent<AbstractCellScript>().SetScale(this.transform.localScale);
        Destroy(this.gameObject);
    }

    public void SetScale(Vector3 scale){
        this.transform.localScale = scale;
    }

}
