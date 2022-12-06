using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class performs controls the drone
/// </summary>
public class DroneScript : MonoBehaviour
{   
    [Header("Drone parameters")]
    [SerializeField] float speed = 5f;

    [Header("Tasks Queue")]
    private Queue<int[]> tasksQueue; // Tasks queue


    /// <summary>
    /// Initializes the tasks queue
    /// </summary>
    private void Awake(){
        tasksQueue = new Queue<int[]>();
    }




}
