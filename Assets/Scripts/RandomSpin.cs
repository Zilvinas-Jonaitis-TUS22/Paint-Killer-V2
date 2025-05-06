using UnityEngine;

public class RandomSpin : MonoBehaviour
{
    public float rotationSpeed = 90f; // Degrees per second
    public float directionChangeInterval = 2f; // Time in seconds between direction changes

    private Vector3 currentAxis;
    private float timer;

    void Start()
    {
        ChooseNewDirection();
    }

    void Update()
    {
        // Rotate the object
        transform.Rotate(currentAxis * rotationSpeed * Time.deltaTime, Space.Self);

        // Countdown to next direction change
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ChooseNewDirection();
        }
    }

    void ChooseNewDirection()
    {
        // Pick a random direction vector with components between -1 and 1
        currentAxis = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        timer = directionChangeInterval;
    }
}
