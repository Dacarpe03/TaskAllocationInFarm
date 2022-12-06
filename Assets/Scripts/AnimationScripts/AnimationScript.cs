/// <summary>
/// Class that makes an animation object moved to its destination
/// </summary>

using System.Collections;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float time = 2f; // Time to get to the destination
    [SerializeField] private float initialPosition = 10f; // Starting height (y) of the object
    [SerializeField] private float finalPosition = 0f; // Destination height (y) of the object
    [SerializeField] private float speed = 2f; // Speed of movement


    /// <summary>
    /// Method that is invoked at the object's instantiation moment
    /// </summary>
    /// <para>
    /// Forces the y position to the initialPosition, calculates the speed and
    /// starts the coroutine that controls when the object is destroyed
    /// </para>
    void Start()
    {
        Vector3 position = this.transform.position;
        position.y = initialPosition;
        this.transform.position = position;
        speed = (initialPosition-finalPosition)/time;
        StartCoroutine(Countdown());
    }


    /// <summary>
    /// Once per frame moves the object at its speed
    /// </summary>
    void Update()
    {
        Vector3 position = this.transform.position;
        position.y -= speed * Time.deltaTime;
        this.transform.position = position;
    }


    /// <summary>
    /// Coroutine that destroys the object once its countdown time is over
    /// </summary>
    private IEnumerator Countdown(){
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
