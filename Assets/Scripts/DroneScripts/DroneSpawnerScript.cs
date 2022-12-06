using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class spawns drones
/// </summary>
public class DroneSpawnerScript : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] int numberOfDrones = 6;
    [SerializeField] GameObject droneObjet;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 initPosition = new Vector3(0f, 0f, 0f);
        for (int i=0; i<numberOfDrones; i++){
            Instantiate(droneObjet, initPosition, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
