using UnityEngine;

public class RingSpin : MonoBehaviour
{
    public float spinSpeed = 100f;

    void Update()
    {
        transform.RotateAround(transform.parent.position, Vector3.up, spinSpeed * Time.deltaTime);
    }
}