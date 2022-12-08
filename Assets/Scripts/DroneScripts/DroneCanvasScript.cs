using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


/// <summary>
/// This class will change the canvas of the drone according to its current state
/// </summary>
public class DroneCanvasScript : MonoBehaviour
{
    [Header("Reference to drone script")]
    private DroneScript myDrone;

    [Header("Text components references")]
    [SerializeField] private TextMeshProUGUI stateText;
    private string[] possibleActivities = {"Resting", "Planting", "Watering", "Harvesting"};
    

    /// <summary>
    /// Gets a reference to the drone script
    /// </summary>
    private void Start(){
        myDrone = GetComponentInParent<DroneScript>();
    }


    /// <summary>
    /// Updates the canvas once per frame based on the drone information
    /// </summary>
    private void Update()
    {
        UpdateActivityText();
    }


    /// <summary>
    /// Updates the activity text based on the drone activity
    /// </summary>
    private void UpdateActivityText(){
        
        string currentActivity = possibleActivities[0];
        if(IsWorking()){
            int activity = GetCurrentTask();
            currentActivity= possibleActivities[activity];
        }
        stateText.text = currentActivity;
    }


    /// <summary>
    /// Gets the current activity of the drone
    /// </summary>
    /// <returns>
    /// The integer that identifies the drone activity
    /// </returns>
    private int GetCurrentTask(){
        return myDrone.GetCurrentTask();
    }


    /// <summary>
    /// Tells if the drone is currently
    /// <summary>
    private bool IsWorking(){
        return myDrone.IsWorking();
    }
}
