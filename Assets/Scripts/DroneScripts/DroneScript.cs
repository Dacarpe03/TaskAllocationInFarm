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
            GetNextTask();
        }
    }

    /// <summary>
    /// Adds a task to the queue
    /// </summary>
    public void AddTask(int[] newTask){
        tasksQueue.Enqueue(newTask);
    }


    private void GetNextTask(){
        int[] nextTask = tasksQueue.Dequeue();
        
    }


    /// <summary>
    /// Moves to the correspoding cell
    /// </summary>
    private IEnumerator MoveToTaskPosition(){
        if(Vector3.Distance(this.transform.position, desiredPosition) > 0.1f){
            this.transform.position = Vector3.MoveTowards(this.transform.position, desiredPosition, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            StartCoroutine(MoveToTaskPosition());
        }else{
            yield return null;
            working = false;
        }
        
    }


}
