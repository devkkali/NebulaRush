using UnityEngine;

public class SpaceshipSpawn : MonoBehaviour
{
	public GameObject enemyPrefab; // Assign this in the Inspector
	public float spawnRate = 2f; // The rate at which enemies will spawn (every 2 seconds by default)

	private Camera mainCamera;
	private float nextSpawnTime;


	private void Start()
	{
		mainCamera = Camera.main; // Cache the main camera
		nextSpawnTime = Time.time + spawnRate; // Initialize the next spawn time
	}

	private void Update()
	{
		// Check if it's time to spawn a new enemy
		if (Time.time >= nextSpawnTime)
		{
			SpawnEnemy();
			nextSpawnTime = Time.time + spawnRate; // Set the time for the next spawn
		}
	}

	private void SpawnEnemy()
	{
		// Determine the vertical bounds of the camera view
		var screenHalfHeight = mainCamera.orthographicSize;
		var verticalExtent = screenHalfHeight * 2f; //will this be used? if not, remove it // bro it might be used later so i kept it thinking of future use at last on the time of submission if not use then we will delete

		// Determine the horizontal bounds of the camera view
		var screenHalfWidth = screenHalfHeight * mainCamera.aspect;
		var horizontalExtent = screenHalfWidth * 2f; //will this be used? if not, remove it

		// Get the enemy renderer component
		var enemyRenderer = enemyPrefab.GetComponent<Renderer>();

		if (enemyRenderer is null)
		{
			Debug.LogError("Enemy prefab must have a Renderer component for accurate width calculation.");
			return;
		}

		// Calculate the maximum spawn area considering the enemy size
		var bounds = enemyRenderer.bounds;
		var maxSpawnX = screenHalfWidth - bounds.extents.x;
		var maxSpawnY = screenHalfHeight - bounds.extents.y;

		// Set spawn position on the extreme right y-axis border
		var spawnX = maxSpawnX; 
		var spawnY = Random.Range(-maxSpawnY, maxSpawnY);

		// Calculate spawn position with an offset from the center of the screen
		var spawnPosition = new Vector3(mainCamera.transform.position.x + spawnX, spawnY, 0);

		var enemyRotation = Quaternion.Euler(0, 0, -90); // Adjust the Euler angles as needed for your prefab
		Instantiate(enemyPrefab, spawnPosition, enemyRotation);
	}
}
