using System.Collections;
using UnityEngine;

/// <summary>
/// This class implements the common functions of a cell
/// </summary>
public abstract class AbstractCellScript : MonoBehaviour
{
    [Header("NextObject")]
    [SerializeField] protected GameObject nextCellStateObject; /// The next cell object that spawns once this cell's task is completed

    [Header("AnimationObject")]
    [SerializeField] protected GameObject animationObject; /// The animation object to spawn when the cell's taks starts
    [SerializeField] protected float initialY; /// The height of the animation object

    [Header("Timers")]
    [SerializeField] protected float timeToNextState = 2f; /// The time to complete this cell's task

    [Header("Manager reference")]
    protected ManagerScript manager; /// The manager where to post the cell's task

    [Header("Cell Types")] /// Cell type to help the manager classify cells
    protected int emptyCell = 1; 
    protected int seedCell = 2;
    protected int wateredCell = 3;

    [Header("Cell id")] /// Cell id assigned by the manager when suscribed to it
    protected int cellId = -1;


    /// <summary>
    /// This method suscribes the cell to the manager
    /// </summary>
    protected virtual void Start(){
        manager = FindObjectOfType<ManagerScript>();
        Suscribe();
        TriggerNextState();
    }


    /// <summary>
    /// Starts the animation process and the countdown to spawn the next state cell
    /// </summary>
    protected virtual void TriggerNextState(){
        StartAnimation();
        StartCoroutine(Countdown());
    }


    /// <summary>
    /// Instantiates the corresponding animation object
    /// </summary>
    protected void StartAnimation(){
        Vector3 position = new Vector3(this.transform.position.x, initialY, this.transform.position.z);
        GameObject anim = Instantiate(animationObject, position, Quaternion.identity);
        anim.transform.localScale = this.transform.localScale;
    }


    /// <summary>
    /// Starts the coundown to destroy this cell and spawn next, the time it takes to do
    /// this is the time to complete the cell's task
    /// </summary>
    protected IEnumerator Countdown(){
        yield return new WaitForSeconds(timeToNextState);
        GameObject newState = Instantiate(nextCellStateObject, this.transform.position, Quaternion.identity);
        newState.GetComponent<AbstractCellScript>().SetScale(this.transform.localScale);
        Unsuscribe();
        Destroy(this.gameObject);
    }


    /// <summary>
    /// Method to adjust the scale based on the number of cells of the grid
    /// </summary>
    /// <param name="scale">
    /// Vector3 parameter that sets the scale of this object
    /// </param>
    public void SetScale(Vector3 scale){
        this.transform.localScale = scale;
    }

    
    /// <summary>
    /// Abstract method that will suscribe on the corresponding list of the manager
    /// depending on the cell type and gets the cell id assigned by the manager.
    /// </summary>
    protected abstract void Suscribe();


    /// <summary>
    /// Abstract method to unsuscribe from the manager when this cell's task is completed
    /// </summary>
    protected abstract void Unsuscribe();

}
