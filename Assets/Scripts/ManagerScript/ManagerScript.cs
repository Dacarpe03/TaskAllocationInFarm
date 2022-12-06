using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    [Header("Cells dictionary")]
    Dictionary<int, AbstractCellScript> emptDict;
    Dictionary<int, AbstractCellScript> seedsDict;
    Dictionary<int, AbstractCellScript> wateredDict;
    int cellsCount = 0;

    [Header("Cell Types")]
    protected int emptyCell = 1;
    protected int seedCell = 2;
    protected int wateredCell = 3;

    void Awake(){
        emptDict = new Dictionary<int, AbstractCellScript>();
        seedsDict = new Dictionary<int, AbstractCellScript>();
        wateredDict = new Dictionary<int, AbstractCellScript>();
    }   
    
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
}
