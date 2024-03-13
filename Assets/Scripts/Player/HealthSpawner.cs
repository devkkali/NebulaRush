using UnityEngine;

public class HealthSpwaner : MonoBehaviour
{
	public GameObject healthspawnPrefab; // Assign this in the Inspector

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
			SpawnHealth();
			nextSpawnTime = Time.time + spawnRate; // Set the time for the next spawn
		}

	}

	private void SpawnHealth()
	{
		// Determine the vertical bounds of the camera view
		var screenHalfHeight = mainCamera.orthographicSize;

		// Determine the horizontal bounds of the camera view
		var screenHalfWidth = screenHalfHeight * mainCamera.aspect;

		// Get the health spawn renderer component
		var healthspawnRenderer = healthspawnPrefab.GetComponent<Renderer>();

		if (healthspawnRenderer is null)
		{
			Debug.LogError("Health spawn prefab must have a Renderer component for accurate width calculation.");
			return;
		}

		// Calculate the maximum spawn area considering the health spawn prefab size
		var bounds = healthspawnRenderer.bounds;
		var maxSpawnX = screenHalfWidth - bounds.extents.x;
		var maxSpawnY = screenHalfHeight - bounds.extents.y;

		// Randomly choose whether to spawn from top, bottom, or right side
		var spawnSide = Random.Range(0, 3);

		float spawnX, spawnY;
		var healthspawnVelocity = Vector3.zero;

		spawnX = maxSpawnX;
		spawnY = Random.Range(-maxSpawnY, maxSpawnY);

		healthspawnVelocity = new Vector2(-2f, 0);

		var spawnPosition = new Vector3(mainCamera.transform.position.x + spawnX, spawnY, 0);

		// Instantiate health spawn
		GameObject health = Instantiate(healthspawnPrefab, spawnPosition, Quaternion.identity);

		// Set velocity for the health spawn to move towards the left
		var healthspawnRigidbody = health.GetComponent<Rigidbody2D>(); // Assuming you have a Rigidbody2D on the healthspawnPrefab
		if (healthspawnRigidbody != null)
		{
			// Adjust the velocity based on the random angle
			healthspawnRigidbody.velocity = healthspawnVelocity;
		}
		else
		{
			Debug.LogError("Health Spawn prefab must have a Rigidbody2D component for velocity control.");
		}
		Destroy(health, 10f);

	}
}
