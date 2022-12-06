using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCellScript : MonoBehaviour
{
    [Header("NextObject")]
    [SerializeField] protected GameObject nextCellStateObject;

    [Header("AnimationObject")]
    [SerializeField] protected GameObject animationObject;
    [SerializeField] protected float initialY;

    [Header("Timers")]
    [SerializeField] protected float timeToNextState = 2f;

    [Header("Manager reference")]
    protected ManagerScript manager;
    protected CellSpawnerScript a;

    [Header("Cell Types")]
    protected int emptyCell = 1;
    protected int seedCell = 2;
    protected int wateredCell = 3;

    [Header("Cell id")]
    protected int cellId = -1;

    protected virtual void Start(){
        manager = FindObjectOfType<ManagerScript>();
        Suscribe();
        TriggerNextState();
    }

    protected void CreateTask(){
    }

    protected virtual void TriggerNextState(){
        StartAnimation();
        StartCoroutine(Countdown());
    }

    protected void StartAnimation(){
        Vector3 position = new Vector3(this.transform.position.x, initialY, this.transform.position.z);
        GameObject anim = Instantiate(animationObject, position, Quaternion.identity);
        anim.transform.localScale = this.transform.localScale;
    }

    protected IEnumerator Countdown(){
        yield return new WaitForSeconds(timeToNextState);
        GameObject newState = Instantiate(nextCellStateObject, this.transform.position, Quaternion.identity);
        newState.GetComponent<AbstractCellScript>().SetScale(this.transform.localScale);
        //Unsuscribe();
        Destroy(this.gameObject);
    }

    public void SetScale(Vector3 scale){
        this.transform.localScale = scale;
    }

    protected abstract void Suscribe();

}
