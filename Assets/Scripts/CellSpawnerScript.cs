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
    [SerializeField] int cell_rows = 10;
    [SerializeField] int cell_columns = 10;


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
        
    }
}
