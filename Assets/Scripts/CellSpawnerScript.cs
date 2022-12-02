using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellSpawnerScript : MonoBehaviour
{   
    [Header("BackgroundObjects")]
    [SerializeField] GameObject field;
    [SerializeField] GameObject grass;
     
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

    // Update is called once per frame
    void Update()
    {
        
    }


    private void CreateCells(){
        float initial_x_position = -totalFieldWidth/2;
        float initial_z_position = -totalFieldWidth/2;
        float y_position = 0;

        float row_offset = totalFieldWidth/cellRows;
        float column_offset = totalFieldWidth/cellColumns;

        for (int row=0; row<cellRows; row++){
            for (int column=0; column<cellColumns; column++){
                float x_position = initial_x_position + row * row_offset;
                float z_position = initial_z_position + column * column_offset;
                CreateCell(x_position, y_position, z_position);
            }
        }
        

    }


    private void CreateCell(float x, float y, float z){
        Vector3 position = new Vector3(x, y, z);
    }
}
