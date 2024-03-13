using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public GameObject enemyPrefab; // Assign this in the Inspector
	public float spawnRate = 5f; // The rate at which enemies will spawn (every 2 seconds by default)
	public float enemySpeed = 3f; // Speed of the enemy

	private Camera mainCamera;
	private float nextSpawnTime;
	private Transform playerTransform;


	private void Start()
	{
		mainCamera = Camera.main; // Cache the main camera
		playerTransform = FindObjectOfType<SpaceshipControls>().transform; // Assuming you have a PlayerControls script
		/*
		 * TODO:
		 * What if you don't?
		 * What if there are multiple objects that have SpaceshipControls on it?
		 * A reference manager would do this job at its best.
		 */
		

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
		var verticalExtent = screenHalfHeight * 2f;

		// Determine the horizontal bounds of the camera view
		var screenHalfWidth = screenHalfHeight * mainCamera.aspect;
		var horizontalExtent = screenHalfWidth * 2f;

		// Get the enemy renderer component
		var enemyRenderer = enemyPrefab.GetComponent<Renderer>(); //TODO: TryGetComponent!!!

		if (enemyRenderer is null)
		{
			Debug.LogError("Enemy prefab must have a Renderer component for accurate width calculation.");
			return;
		}

		// Calculate the maximum spawn area considering the enemy size
		var bounds = enemyRenderer.bounds;
		var maxSpawnX = screenHalfWidth - bounds.extents.x;
		var maxSpawnY = screenHalfHeight - bounds.extents.y;

		// Choose a random position within the spawn area
		var spawnX = Random.Range(0, maxSpawnX);
		var spawnY = Random.Range(-maxSpawnY, maxSpawnY);

		// Calculate spawn position with an offset from the center of the screen
		var spawnPosition = new Vector3(mainCamera.transform.position.x + spawnX, spawnY, 0);

		var enemyRotation = Quaternion.Euler(0, 0, -90); // Adjust the Euler angles as needed for your prefab
		var newEnemy = Instantiate(enemyPrefab, spawnPosition, enemyRotation);

		// Get the EnemyMovement component from the instantiated enemy
		var enemyMovement = newEnemy.GetComponent<EnemyMovement>(); //TODO: TryGetComponent!!!
		if (enemyMovement is not null)
		{
			// Set the target player for the enemy to follow
			enemyMovement.SetTarget(playerTransform);
		}
		else
		{
			Debug.LogError("Enemy prefab must have an EnemyMovement component for movement.");
		}
	}
}
