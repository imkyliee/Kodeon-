using UnityEngine;

public class DaylightCycle : MonoBehaviour
{
    public float dayDurationInSeconds = 120f; 
    void Update()
    {
        float rotationSpeed = 360f / dayDurationInSeconds;
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }
}