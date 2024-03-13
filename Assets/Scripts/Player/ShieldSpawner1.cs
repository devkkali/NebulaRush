using UnityEngine;

public class ShieldSpwaner : MonoBehaviour
{
    public GameObject shieldspawnPrefab; // Assign this in the Inspector

    [SerializeField] private float spawnRate = 10f;

    private Camera mainCamera;
    private float nextSpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main; // Cache the main camera
        nextSpawnTime = Time.time + spawnRate; // Initialize the next spawn time
    }

    // Update is called once per frame
    void Update()
    {
        // Check if it's time to spawn a new enemy
        if (Time.time >= nextSpawnTime)
        {
            SpawnShield();
            nextSpawnTime = Time.time + spawnRate; // Set the time for the next spawn
        }
    }

    private void SpawnShield()
    {
        // Determine the vertical bounds of the camera view
        var screenHalfHeight = mainCamera.orthographicSize;

        // Determine the horizontal bounds of the camera view
        var screenHalfWidth = screenHalfHeight * mainCamera.aspect;

        // Get the Shield spawn renderer component
        var shieldspawnRenderer = shieldspawnPrefab.GetComponent<Renderer>();

        if (shieldspawnRenderer is null)
        {
            Debug.LogError("Shield spawn prefab must have a Renderer component for accurate width calculation.");
            return;
        }

        // Calculate the maximum spawn area considering the shield spawn prefab size
        var bounds = shieldspawnRenderer.bounds;
        var maxSpawnX = screenHalfWidth - bounds.extents.x;
        var maxSpawnY = screenHalfHeight - bounds.extents.y;

        // Randomly choose whether to spawn from top, bottom, or right side
        var spawnSide = Random.Range(0, 3);

        float spawnX, spawnY;
        var shieldspawnVelocity = Vector3.zero;

        spawnX = maxSpawnX;
        spawnY = Random.Range(-maxSpawnY, maxSpawnY);

        shieldspawnVelocity = new Vector2(-2f, 0);

        var spawnPosition = new Vector3(mainCamera.transform.position.x + spawnX, spawnY, 0);

        // Instantiate shield spawn
        GameObject shield = Instantiate(shieldspawnPrefab, spawnPosition, Quaternion.identity);

        // Set velocity for the shield spawn to move towards the left
        var shieldspawnRigidbody =
            shield.GetComponent<Rigidbody2D>(); // Assuming you have a Rigidbody2D on the shieldspawnPrefab
        if (shieldspawnRigidbody != null)
        {
            // Adjust the velocity based on the random angle
            shieldspawnRigidbody.velocity = shieldspawnVelocity;
        }
        else
        {
            Debug.LogError("Shield Spawn prefab must have a Rigidbody2D component for velocity control.");
        }
        Destroy(shield, 10f);

    }
}