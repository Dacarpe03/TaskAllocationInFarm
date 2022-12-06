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

    /// <summary>
    /// Spawns the drones
    /// </summary>
    void Start()
    {
        Vector3 initPosition = new Vector3(0f, 0f, 0f);
        for (int i=0; i<numberOfDrones; i++){
            Instantiate(droneObjet, initPosition, Quaternion.identity);
        }
    }
}
