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

    [Header("Keep memory of the last element in the queue")]
    private int lastCellId = -1;
    private int lastTask = -1;


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
    [SerializeField] private float minThreshold = 5.5f;
    [SerializeField] private float maxThreshold = 40f;
    [SerializeField] private float reduceRate = 7.85f;
    [SerializeField] private float increaseRate = 17.73f;
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
        taskThresholds.Add(plantTask, minThreshold);
        taskThresholds.Add(waterTask, minThreshold);
        taskThresholds.Add(harvestTask, minThreshold);
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
        ReduceThreshold(newTask[0]);
        lastTask = newTask[0];
        lastCellId = newTask[1];
    }


    /// <summary>
    /// Reduces the threshold for a given task
    /// </summary>
    /// <param name="taskType">
    /// Int. The task type indicating the threshold to reduce
    /// </param>
    public void ReduceThreshold(int taskType){
        taskThresholds[taskType] -= reduceRate;
        if (taskThresholds[taskType] <= minThreshold){
            taskThresholds[taskType] = minThreshold;
        }
    }


    /// <summary>
    /// Increases the threshold for a given task
    /// </summary>
    /// <param name="taskType">
    /// Int. The task type indicating the threshold to increase
    /// </param>
    public void IncreaseThreshold(int taskType){
        taskThresholds[taskType] += increaseRate;
        if (taskThresholds[taskType] >= maxThreshold){
            taskThresholds[taskType] = maxThreshold;
        }
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
            manager.IncreaseChanges();
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
        int cellId = task[1];
        float bid = 1f;
        float demandSquared = Mathf.Pow(taskDemand, 2);
        float thresholdSquared = Mathf.Pow(taskThresholds[taskType], 2);
        float timeComponent = GetTimeComponent(taskType, cellId);
        float timeComponentSquared = Mathf.Pow(taskThresholds[taskType], 2*beta);
        float probability = demandSquared/(demandSquared + alpha*thresholdSquared + timeComponentSquared);
        return bid;
    }


    /// <summary>
    /// Gets the time component from the bidding system
    /// </summary>
    /// <param name="taskType">
    /// Int. Represents the type of the task
    /// </param>
    /// <param name="cellId">
    /// Int. The cell id that will be used to calculate the distance to it asking the manager
    /// </param>
    /// <returns>
    /// Float. The time component from the bidding formula
    /// </returns>
    private float GetTimeComponent(int taskType, int cellId){
        float taskTime = 2f;

        int numberOfTasksInQueue = tasksQueue.Count;
        int numberOfChanges = this.GetNumberOfChanges();

        float timeToReach = CalculateTimeToReachCell(taskType, cellId);

        float timeComponent = taskTime * numberOfTasksInQueue + (numberOfChanges * changeTime * rechargeTime) + timeToReach;
        return 2f;
    }


    /// <summary>
    /// Counts the number of changes to perform a task
    /// </summary>
    /// <returns>
    /// Int. The number of changes that the drone will do in the next tasks
    /// </returns>
    private int GetNumberOfChanges(){
        int numberOfChanges = 0;
        int currentTaskType = currentTask;
        foreach (int[] nextTaskArray in tasksQueue){
            int next = nextTaskArray[0];
            if (currentTaskType != next){
                currentTaskType = next;
                numberOfChanges += 1;
            }
        }
        return numberOfChanges;
    }


    /// <summary>
    /// Calculates the time to get to a cell after being in the last cell
    /// </summary>
    /// <returns>
    /// Float. Time to get to a cell from the last cell in the queue
    /// </returns>
    private float CalculateTimeToReachCell(int taskType, int cellId){
        float timeToReach = 0f;
        if (lastCellId != -1){
            Vector3 lastCellPosition = manager.GetPositionOfCell(lastTask, lastCellId);
            if (lastCellPosition.Equals(Vector3.zero)){
                lastCellPosition = this.transform.position;
            }
            Vector3 auctionTaskPosition = manager.GetPositionOfCell(taskType, cellId);
            float distance = Vector3.Distance(lastCellPosition, auctionTaskPosition);
            timeToReach = distance/speed;
        }

        return timeToReach;
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
        manager.AddHarvest(maxResources-currentResources);
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


    /// <summary>
    /// Function that the cavas will call to retrieve threshold info to update sliders
    /// </summary>
    /// <returns>
    /// Dictionary with key as int and value as float representing each threshold
    /// </returns>
    public Dictionary<int,float> GetThresholds(){
        return taskThresholds;
    }
}
