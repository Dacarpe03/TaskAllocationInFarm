using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;


/// <summary>
/// Class to make a comunication tabloid between cells and drones
/// </summary>
public class ManagerScript : MonoBehaviour
{
    [Header("Time to report")]
    private float timeToReport = 10; // Time interval between countings


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
    private Queue<int[]> tasks;


    [Header("Save file location")]
    private string dirPath = "";
    private string fileName = "myfile.csv";

    /// <summary>
    /// First method to be invoked, will initialize the dictionaries and tasks queue
    /// </summary>
    void Awake(){
        emptDict = new Dictionary<int, AbstractCellScript>();
        seedsDict = new Dictionary<int, AbstractCellScript>();
        wateredDict = new Dictionary<int, AbstractCellScript>();
        tasks = new Queue<int[]>();
        dirPath = Application.dataPath;
    }


    /// <summary>
    /// Begins the constant reporting
    /// </summary>
    void Start(){
        drones = FindObjectsOfType<DroneScript>();
        Debug.Log(drones.Length);
        StartCoroutine(SaveFile());
    }


    /// <summary>
    /// Each frame if there is a new task, starts a new auction
    /// </summary>
    void Update(){
        if (tasks.Count > 0){
            Debug.Log("offering task");
            int[] taskOffered = tasks.Dequeue();
            AuctionTask(taskOffered);
        }
    }


    /// <summary>
    /// Method to start an auction between the drones to win the task
    /// </summary>
    /// <param name="taskOffered">
    /// int[] with the task data
    /// </param>
    private void AuctionTask(int[] taskOffered){
        float[] bids = new float[drones.Length];
        float maxBid = 0;

        for (int i=0; i<drones.Length; i++){
            float currentBid = drones[i].Bid(taskOffered, 20);
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

        if (participants.Count > 0){
            int raffleWinnerIndex = UnityEngine.Random.Range(0, participants.Count);
            int raffleWinner = participants[raffleWinnerIndex];
            drones[raffleWinner].AddTask(taskOffered);
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
            string dataToStore = nEmptyCells.ToString() + "," + nSeedCells.ToString() + "," + nWateredCells.ToString() + "\n";

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
        tasks.Enqueue(newTask);
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
            emptDict[cellsCount] = newCell;
        } else if (type==seedCell){
            seedsDict[cellsCount] = newCell;
        }else if (type==wateredCell){
            wateredDict[cellsCount] = newCell;
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
        if (taskType==emptyCell){
            return emptDict[cellId].GetPosition();
        } else if (taskType==seedCell){
            return seedsDict[cellId].GetPosition();
        }else if (taskType==wateredCell){
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

    
}
