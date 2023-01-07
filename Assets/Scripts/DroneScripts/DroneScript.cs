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
    [SerializeField] private float rechargeTime = 2f;
    [SerializeField] private float changeTime = 2f;


    [Header("Tasks ids")]
    private int plantTask = 1;
    private int waterTask = 2;
    private int harvestTask = 3;


    [Header("Attributes to control task")]
    private Vector3 desiredPosition = new Vector3(10f, 10f, 10f);
    private Dictionary<int, Vector3> rechargePositions;

    private bool working = false;
    private bool recharging = false;
    private bool changing = false;

    private bool firstTask = true;

    private int currentTask = 0;
    private int currentCell = -1;
    private int maxResources = 3;
    private int currentResources = 3;


    [Header("Tasks thresholds")]
    [SerializeField] private float minThreshold;
    [SerializeField] private float maxThreshold;
    private Dictionary<int, float> taskThresholds;


    [Header("Bidding parameters")]
    private float alpha = 2f;
    private float beta = 5f;

    [Header("Tasks Queue")]
    private Queue<int[]> tasksQueue; // Tasks queue


    [Header("Manager reference")]
    private ManagerScript manager;


    /// <summary>
    /// Initializes the tasks queue
    /// </summary>
    private void Awake(){
        tasksQueue = new Queue<int[]>();
        Vector3 seedPosition = new Vector3(-4f, height, 15f);
        Vector3 wellPosition = new Vector3(4f, height, 15f);
        Vector3 carPosition = new Vector3(15f, height, 0f);

        rechargePositions = new Dictionary<int, Vector3>();
        rechargePositions.Add(plantTask, seedPosition);
        rechargePositions.Add(waterTask, wellPosition);
        rechargePositions.Add(harvestTask, carPosition);

        taskThresholds = new Dictionary<int, float>();
        taskThresholds.Add(plantTask, maxThreshold);
        taskThresholds.Add(waterTask, maxThreshold);
        taskThresholds.Add(harvestTask, maxThreshold);
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
        if (!working && !recharging && !changing && tasksQueue.Count>0){
            Debug.Log("Next task");
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

        desiredPosition = manager.GetPositionOfCell(nextTask[0], nextTask[1]);
        desiredPosition.y = height;
        currentCell = nextTask[1];

        if (currentTask != nextTask[0] && !firstTask){
            StartCoroutine(ChangeTask(nextTask[0], false));
        }else{
            currentTask = nextTask[0];
            if (currentResources > 0){
                StartCoroutine(MoveToTaskPosition());
            }else{
                StartCoroutine(MoveToRechargePosition(currentTask));
            }
        }
    }


    /// <summary>
    /// Functions that changes the task
    /// </summary>
    private IEnumerator ChangeTask(int nextTask, bool dropped){
        changing = true;
        if (currentTask == harvestTask && !dropped){
            //Debug.Log("Going to drop harvest");
            StartCoroutine(DropHarvest(nextTask));
        }else{
            currentTask = nextTask;
            if (nextTask == harvestTask){
                //Debug.Log("Going to harvest");
                currentResources = maxResources;
                yield return new WaitForSeconds(changeTime);
                working = true;
                changing = false;
                StartCoroutine(MoveToTaskPosition());
            }else{
                //Debug.Log("Going to change");
                currentResources = 0;
                yield return new WaitForSeconds(changeTime);
                recharging = true;
                changing = false;
                StartCoroutine(MoveToRechargePosition(currentTask));
            }
            //Debug.Log("Changed");
        }
        
    }


    /// <summay>
    /// Gets the bid
    /// </summary>
    /// <param name="task">
    /// task is an integer array. The first position is the task type, the second position the cell id that is requesting the task.
    /// </param>
    /// <param name="taskDemand">
    /// The demand of the task type
    /// </param>
    /// <returns>
    /// Float. The bid made for the task
    /// </returns>
    public float Bid(int[] task, float taskDemand){
        int taskType = task[0];
        float bid = 1f;
        float demandSquared = Mathf.Pow(taskDemand, 2);
        float thresholdSquared = Mathf.Pow(taskThresholds[taskType], 2);
        float timeComponent = GetTimeComponent(taskType);
        float timeComponentSquared = Mathf.Pow(taskThresholds[taskType], 2*beta);
        float probability = demandSquared/(demandSquared + alpha*thresholdSquared + timeComponentSquared);
        return bid;
    }


    /// <summary>
    /// Gets the time component from the bidding system
    /// </summary>
    private float GetTimeComponent(int taskType){
        return 2f;
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
    /// Moves to recharge position
    /// </summary>
    private IEnumerator MoveToRechargePosition(int taskType){
        //Debug.Log("Going to recharge");
        recharging = true;
        Vector3 rechargePosition = rechargePositions[taskType];
        if(Vector3.Distance(this.transform.position, rechargePosition) > 0.1f){
            this.transform.position = Vector3.MoveTowards(this.transform.position, rechargePosition, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            StartCoroutine(MoveToRechargePosition(taskType));
        }else{
            StartCoroutine(Recharge());
            yield return null;
        }
    }


    /// <summary>
    /// Goes to the car and drops the harvest before changing task
    /// </summary>
    private IEnumerator DropHarvest(int nextTask){
        Vector3 rechargePosition = rechargePositions[harvestTask];
        if(Vector3.Distance(this.transform.position, rechargePosition) > 0.1f){
            this.transform.position = Vector3.MoveTowards(this.transform.position, rechargePosition, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            StartCoroutine(DropHarvest(nextTask));
        }else{
            //Debug.Log("Got to the dropout");
            StartCoroutine(Discharge(nextTask));
            yield return null;
        }
    }

    
    /// <summary>
    /// Triggers the task
    /// </summary>
    private IEnumerator DoTask(){
        float waitTime = manager.TriggerTask(currentTask, currentCell);
        currentResources -= 1;
        yield return new WaitForSeconds(waitTime);
        working = false;
        firstTask = false;
    }


    /// <summary>
    /// Recharges to max capacity
    /// </summary>
    private IEnumerator Recharge(){
        currentResources = maxResources;
        working = true;
        yield return new WaitForSeconds(rechargeTime);
        recharging = false;
        StartCoroutine(MoveToTaskPosition());
    }

    /// <summary>
    /// Discharges harvest
    /// </summary>
    private IEnumerator Discharge(int nextTask){
        currentResources = maxResources;
        working = true;
        yield return new WaitForSeconds(rechargeTime);
        recharging = false;
        StartCoroutine(ChangeTask(nextTask, true));
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
    /// Gets the current activity of the drone
    /// </summary>
    /// <returns>
    /// The number of resources left
    /// </returns>
    public int GetCurrentResources(){
        return currentResources;
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


    /// <summary>
    /// Tells if the drone is currently
    /// <summary>
    /// <returns>
    /// True if the drone is recharging
    /// </returns>
    public bool IsRecharging(){
        return recharging;
    }


    /// <summary>
    /// Tells if the drone is changing task
    /// </summary>
    /// <returns>
    /// True if the drone is changing task
    /// </returns>
    public bool IsChanging(){
        return changing;
    }

}
