using UnityEngine;

public class RandomRotator : MonoBehaviour
{
    [SerializeField]
    private float tumble;

    void Start()
    {
        // Use Rigidbody2D for 2D physics
        var rigidbody2D = GetComponent<Rigidbody2D>();

        // Apply a random rotation around the z-axis (2D rotation)
        rigidbody2D.angularVelocity = Random.Range(-tumble, tumble);
    }
    
    
    
    void Update()
    {
        // Destroy the bullet if it goes out of the screen
        if (!GetComponent<Renderer>().isVisible)
        {
            Destroy(gameObject);
        }
    }
}