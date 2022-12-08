using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class performs controls the drone
/// </summary>
public class DroneScript : MonoBehaviour
{   
    [Header("Drone parameters")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float height = 5f;

    [Header("Attributes to control task")]
    private Vector3 desiredPosition = new Vector3(10f, 10f, 10f);
    private bool working = false;
    private int currentTask = 0;
    private int currentCell = -1;


    [Header("Tasks Queue")]
    private Queue<int[]> tasksQueue; // Tasks queue

    [Header("Manager reference")]
    private ManagerScript manager;


    /// <summary>
    /// Initializes the tasks queue
    /// </summary>
    private void Awake(){
        tasksQueue = new Queue<int[]>();
    }


    /// <summary>
    /// Finds a reference to the manager when the drown is instantiated
    /// </summary>
    private void Start(){
        manager = FindObjectOfType<ManagerScript>();
    }


    /// <summary>
    /// Checks if the drone can perform a new task
    /// </summary>
    private void Update(){
        if (!working && tasksQueue.Count>0){
            Debug.Log("next task");
            GetNextTask();
        }
    }


    /// <summary>
    /// Adds a task to the queue
    /// </summary>
    public void AddTask(int[] newTask){
        tasksQueue.Enqueue(newTask);
    }


    /// <summary>
    /// Gets next task
    /// </summary>
    private void GetNextTask(){
        int[] nextTask = tasksQueue.Dequeue();
        currentTask = nextTask[0];
        currentCell = nextTask[1];
        desiredPosition = manager.GetPositionOfCell(currentTask, currentCell);
        desiredPosition.y = height;
        StartCoroutine(MoveToTaskPosition());
    }


    /// <summay>
    /// Gets the bid
    /// </summary>
    public float Bid(int[] task){
        return 1f;
    }


    /// <summary>
    /// Moves to the correspoding cell
    /// </summary>
    private IEnumerator MoveToTaskPosition(){
        working = true;
        if(Vector3.Distance(this.transform.position, desiredPosition) > 0.1f){
            this.transform.position = Vector3.MoveTowards(this.transform.position, desiredPosition, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            StartCoroutine(MoveToTaskPosition());
        }else{
            StartCoroutine(DoTask());
            yield return null;
        }
        
    }


    /// <summary>
    /// Triggers the task
    /// </summary>
    private IEnumerator DoTask(){
        float waitTime = manager.TriggerTask(currentTask, currentCell);
        yield return new WaitForSeconds(waitTime);
        working = false;
    }


    /// <summary>
    /// Gets the current activity of the drone
    /// </summary>
    /// <returns>
    /// The integer that identifies the drone activity
    /// </returns>
    public int GetCurrentTask(){
        return currentTask;
    }


    /// <summary>
    /// Tells if the drone is currently
    /// <summary>
    /// <returns>
    /// True if the drone is performing a task
    /// </returns>
    public bool IsWorking(){
        return working;
    }

}
