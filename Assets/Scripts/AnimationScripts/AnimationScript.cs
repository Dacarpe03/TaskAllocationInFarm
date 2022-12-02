using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float time = 2f;
    [SerializeField] private float initialPosition = 10f;
    [SerializeField] private float speed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 position = this.transform.position;
        position.y = initialPosition;
        this.transform.position = position;
        speed = initialPosition/time;
        StartCoroutine(Countdown());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = this.transform.position;
        position.y -= speed * Time.deltaTime;
        this.transform.position = position;
    }

    private IEnumerator Countdown(){
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
