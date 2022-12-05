using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellSpawnerScript : MonoBehaviour
{   
    [Header("BackgroundObjects")]
    [SerializeField] GameObject field;
    [SerializeField] GameObject grass;

    [Header("Cells")]
    [SerializeField] GameObject emptyCell;
    [SerializeField] GameObject seedCell;
    [SerializeField] GameObject wateredCell;
     
    [Header("Cell parameters")]
    [SerializeField] float totalFieldWidth = 20;
    [SerializeField] int cellRows = 10;
    [SerializeField] int cellColumns = 10;


    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = new Vector3(totalFieldWidth, totalFieldWidth, 1);
        Vector3 position = this.transform.position;

        Vector3 fieldPosition = new Vector3(position.x, position.y-0.1f, position.z);
        Instantiate(field, fieldPosition, Quaternion.identity);

        Vector3 grassPosition = new Vector3(position.x, position.y-0.2f, position.z);
        Instantiate(grass , grassPosition, Quaternion.identity);

        CreateCells();
    }


    private void CreateCells(){

        float measure = totalFieldWidth-2;
        float initialXPosition = -measure/2;
        float initialZPosition = -measure/2;
        float yPosition = 0;

        float rowOffset = measure/(cellRows-1);
        float columnOffset = measure/(cellColumns-1);

        float xScale = rowOffset * 0.8f;
        float zScale = columnOffset * 0.8f;

        for (int row=0; row<cellRows; row++){
            for (int column=0; column<cellColumns; column++){
                float xPosition = initialXPosition + row * rowOffset;
                float zPosition = initialZPosition + column * columnOffset;
                CreateCell(xPosition, yPosition, zPosition, xScale, zScale);
            }
        }
    }


    private void CreateCell(float xPosition, float yPosition, float zPosition, float xScale, float zScale){
        int type = Random.Range(0, 3);
        Vector3 position = new Vector3(xPosition, yPosition, zPosition);
        Vector3 scale = new Vector3(xScale, 1f, zScale);

        if (type == 0){    
            GameObject a = Instantiate(emptyCell, position, Quaternion.identity);
            a.transform.localScale = scale;
        }else if (type == 1){    
            GameObject a = Instantiate(seedCell, position, Quaternion.identity);
            a.transform.localScale = scale;
        }else if (type == 2){    
            GameObject a = Instantiate(wateredCell, position, Quaternion.identity);
            a.transform.localScale = scale;
        }
    }
}
