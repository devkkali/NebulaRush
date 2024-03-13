using UnityEngine;

public class PowerSpinner : MonoBehaviour
{
    public float rotationSpeed = 100f; // Rotation speed in degrees per second

    void Update()
    {
        // Spin the object around its local Z axis at rotationSpeed degrees per second
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}