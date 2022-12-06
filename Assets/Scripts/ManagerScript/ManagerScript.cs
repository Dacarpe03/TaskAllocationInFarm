using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    [Header("Cell Types")] // To know in which dictionary place the cell
    protected int emptyCell = 1;
    protected int seedCell = 2;
    protected int wateredCell = 3;


    /// <summary>
    /// First method to be invoked, will initialize the dictionaries
    /// </summary>
    void Awake(){
        emptDict = new Dictionary<int, AbstractCellScript>();
        seedsDict = new Dictionary<int, AbstractCellScript>();
        wateredDict = new Dictionary<int, AbstractCellScript>();
    }   
    

    /// <summary>
    /// Begins the constant reporting
    /// </summary>
    void Start(){
        StartCoroutine(ShowInfo());
    }


    /// <summary>
    /// Coroutine that executes in time intervals to follow the progress of the simulation
    /// </summary>
    private IEnumerator ShowInfo(){
        string report = "Empty Cells: " + emptDict.Count.ToString();
        report += "\nSeed Cells:" + seedsDict.Count.ToString();
        report += "\nWatered Cells:" + wateredDict.Count.ToString();
        Debug.Log(report);
        yield return new WaitForSeconds(timeToReport);
        StartCoroutine(ShowInfo());
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

    
}
