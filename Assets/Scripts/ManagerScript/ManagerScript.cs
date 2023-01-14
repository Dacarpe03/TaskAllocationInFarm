using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.IO;
using System.Globalization;

/// <summary>
/// Class to make a comunication tabloid between cells and drones
/// </summary>
public class ManagerScript : MonoBehaviour
{
    [Header("Times")]
    [SerializeField] private float simulationTime = 15*60;
    [SerializeField] private float timeToReport = 5; // Time interval between countings

    [Header("Cells dictionary")]
    Dictionary<int, AbstractCellScript> emptDict; // Empty cells dictionary
    Dictionary<int, AbstractCellScript> seedsDict; // Seed cells dictionary
    Dictionary<int, AbstractCellScript> wateredDict; // Watered cells dictionary
    int cellsCount = 0; // This counter will serve as the id for a new cell


    [Header("Drones list")]
    DroneScript[] drones;


    [Header("Cell/Task Types")] // To know in which dictionary place the cell
    protected int emptyCell = 1;
    protected int seedCell = 2;
    protected int wateredCell = 3;


    [Header("Tasks Queue")]
    private List<int[]> tasks;


    [Header("Save file location")]
    private string dirPath = "";
    private string fileName = "results.csv";
    private string simulationName;

    [Header("Simulation results")]
    private int total_changes = 0;
    private int total_tasks = 0;
    private int total_reloads = 0;

    /// <summary>
    /// First method to be invoked, will initialize the dictionaries and tasks queue
    /// </summary>
    void Awake(){
        emptDict = new Dictionary<int, AbstractCellScript>();
        seedsDict = new Dictionary<int, AbstractCellScript>();
        wateredDict = new Dictionary<int, AbstractCellScript>();
        tasks = new List<int[]>();
        dirPath = Application.dataPath;
    }


    /// <summary>
    /// Begins the constant reporting
    /// </summary>
    void Start(){
        drones = FindObjectsOfType<DroneScript>();
        Debug.Log(drones.Length);
        CreateFile();
        StartCoroutine(SaveFile());
        StartCoroutine(FinishSimulation());
    }


    /// <summary>
    /// Each frame if there is a new task, starts a new auction
    /// </summary>
    void Update(){
        if (tasks.Count > 0){
            Debug.Log("offering task");
            int randomIndex = UnityEngine.Random.Range(0, tasks.Count);
            int[] taskOffered = tasks[randomIndex];
            AuctionTask(taskOffered, randomIndex);
        }
    }


    /// <summary>
    /// Method to start an auction between the drones to win the task
    /// </summary>
    /// <param name="taskOffered">
    /// int[] with the task data
    /// </param>
    private void AuctionTask(int[] taskOffered, int taskId){
        int taskType = taskOffered[0];
        int demand = GetTaskDemand(taskType);

        float[] bids = new float[drones.Length];
        float maxBid = 0;
        for (int i=0; i<drones.Length; i++){
            float currentBid = drones[i].Bid(taskOffered, demand);
            bids[i] = currentBid;
            if (maxBid < currentBid){
                maxBid = currentBid;
            }
        }

        List<int> participants = new List<int>(); 
        for (int i=0; i<bids.Length; i++){
            if(bids[i] == maxBid){
                participants.Add(i);
            }
        }

        if (participants.Count > 0 && maxBid > 0){
            int participantWinnerIndex = UnityEngine.Random.Range(0, participants.Count);
            int raffleWinner = participants[participantWinnerIndex];
            drones[raffleWinner].AddTask(taskOffered);
            for (int i=0; i<drones.Length; i++){
                if (i!=raffleWinner){
                    drones[i].IncreaseThreshold(taskType);
                }
            }
            tasks.RemoveAt(taskId);
        }  
    }


    /// <summary>
    /// Method to get the number of cells given a type
    /// </summary>
    /// <param name"cellType">
    /// The type of the cell to count
    /// </param>
    /// <returns>
    /// Int. The count of the cell type
    /// </returns>
    private int GetTaskDemand(int cellType){
        int demand = 0;
        if (cellType==emptyCell){
            demand = emptDict.Count;
        } else if (cellType==seedCell){
            demand = seedsDict.Count;
        }else if (cellType==wateredCell){
            demand = wateredDict.Count;
        }
        return demand;
    }



    /// <summary>
    /// Method to create the csv file
    /// </summary>
    private void CreateFile(){
        simulationName = System.DateTime.Now.ToString("MM-dd-hh-mm-ss");
        
        string fullPath = Path.Combine(dirPath, fileName);
        try {
            if (!File.Exists(fullPath)){
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                string dataToStore =  "simulation_name,empty_cells,seed_cells,watered_cells,total_changes,total_tasks,total_reloads";
                for (int i=0; i<drones.Length; i++){
                    dataToStore += ",drone" + i.ToString() + "_threshold" + ",drone" + i.ToString() + "_speciality";
                }
                dataToStore += "\n";
                using (FileStream stream = new FileStream(fullPath, FileMode.Append)){
                    using (StreamWriter writer = new StreamWriter(stream)){
                        writer.Write(dataToStore);
                    }
                }
            }
            
        }
        catch (Exception e){
            Debug.LogError("Error ocurred when trying to save to file" + fullPath + "\n" + e);
        }
    }


    /// <summary>
    /// Coroutine that executes in time intervals to follow the progress of the simulation
    /// </summary>
    private IEnumerator SaveFile(){
        int nEmptyCells = emptDict.Count;
        int nSeedCells = seedsDict.Count;
        int nWateredCells = wateredDict.Count;

        string fullPath = Path.Combine(dirPath, fileName);

        try {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = simulationName + "," +
                                 nEmptyCells.ToString() + "," + 
                                 nSeedCells.ToString() + "," + 
                                 nWateredCells.ToString() + "," + 
                                 total_changes.ToString() + "," + 
                                 total_tasks.ToString() + "," + 
                                 total_reloads.ToString();

            for (int i=0; i<drones.Length; i++){
                    float minThreshold = drones[i].GetMinThreshold();
                    int speciality = drones[i].GetSpeciality();
                    dataToStore += "," + minThreshold.ToString("#.00", CultureInfo.InvariantCulture) + "," + speciality.ToString();
            }
            dataToStore += "\n";
            using (FileStream stream = new FileStream(fullPath, FileMode.Append)){
                using (StreamWriter writer = new StreamWriter(stream)){
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e){
            Debug.LogError("Error ocurred when trying to save to file" + fullPath + "\n" + e);
        }
        yield return new WaitForSeconds(timeToReport);
        StartCoroutine(SaveFile());
    }
    

    /// <summary>
    /// Methods to add a task to the queue
    /// </summary>
    /// <param  name="taskType">
    /// The type of task 
    /// </param>
    /// <param name="cellId">
    /// Cell that asks for the task
    /// </param>
    private void AddTask(int taskType, int cellId){
        int[] newTask = new int[]{taskType, cellId};
        tasks.Add(newTask);
    }


    /// <summary> 
    /// Adds a cell to the corresponding dict
    /// </summary>
    /// <param name="newCell">
    /// AbstractCellScript to be added to the dictionary
    /// </parm>
    /// <param name="type">
    /// int to know in which dictionary to store the new cell
    /// </param>
    /// <returns>
    /// int as the cell id for the suscribed cell
    /// </returns>
    public int AddCell(AbstractCellScript newCell, int type){
        if (type==emptyCell){
            emptDict.Add(cellsCount, newCell);
        } else if (type==seedCell){
            seedsDict.Add(cellsCount, newCell);
        }else if (type==wateredCell){
            wateredDict.Add(cellsCount, newCell);
        }

        int cellId = cellsCount;
        AddTask(type, cellId);

        cellsCount += 1;
        return cellId;
    }


    /// <summary> 
    /// Removes a cell from the corresponding dict
    /// </summary>
    /// <param name="newCell">
    /// AbstractCellScript to be removed from the dictionary
    /// </parm>
    /// <param name="type">
    /// int to know from which dictionary remove the cell
    /// </param>
    public void RemoveCell(int cellId, int type){
        if (type==emptyCell){
            emptDict.Remove(cellId);
        } else if (type==seedCell){
            seedsDict.Remove(cellId);
        }else if (type==wateredCell){
            wateredDict.Remove(cellId);
        }
    }


    /// <summary>
    /// Gets the position of a cell give a cell type and its id
    /// </summary>
    /// <param name="taskType">
    /// The number of the dict where to look for
    /// </param>
    /// <param name="cellId">
    /// Cell id of the dictionary
    /// </param>
    public Vector3 GetPositionOfCell(int taskType, int cellId){
        if (taskType==emptyCell && emptDict.ContainsKey(cellId)){
            return emptDict[cellId].GetPosition();
        } else if (taskType==seedCell && seedsDict.ContainsKey(cellId)){
            return seedsDict[cellId].GetPosition();
        }else if (taskType==wateredCell && wateredDict.ContainsKey(cellId)){
            return wateredDict[cellId].GetPosition();
        }

        return Vector3.zero;
    }
    

    /// <summary>
    /// Triggers the cell for it to go to the next state
    /// </summary>
    public float TriggerTask(int taskType, int cellId){
        if (taskType==emptyCell){
            return emptDict[cellId].TriggerNextState();
        } else if (taskType==seedCell){
            return seedsDict[cellId].TriggerNextState();
        }else if (taskType==wateredCell){
            return wateredDict[cellId].TriggerNextState();
        }
        
        return 0f;
    }

    
    /// <summary>
    /// Increases the number of changes made
    /// </summary>
    public void IncreaseChanges(){
        total_changes += 1;
    }


    /// <summary>
    /// Increase total tasks done
    /// </summary>
    public void IncreaseTask(){
        total_tasks += 1;
    }
    

    /// <summary>
    /// Increase total reloads done
    /// </summary>
    public void IncreaseReloads(){
        total_reloads += 1;
    }

    private IEnumerator FinishSimulation(){
        yield return new WaitForSeconds(simulationTime);
        SceneManager.LoadScene(0);
    }
}
