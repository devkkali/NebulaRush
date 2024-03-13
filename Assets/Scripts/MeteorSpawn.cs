using UnityEngine;

public class MeteorSpawn : MonoBehaviour
{
	public GameObject[] meteorPrefabs; // Assign this in the Inspector
	//public float spawnRate = 2f; // The rate at which enemies will spawn (every 2 seconds by default)
	
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
			SpawnMeteor();
			nextSpawnTime = Time.time + spawnRate; // Set the time for the next spawn
		}

	}

	private void SpawnMeteor()
	{
		// Determine the vertical bounds of the camera view
		var screenHalfHeight = mainCamera.orthographicSize;

		// Determine the horizontal bounds of the camera view
		var screenHalfWidth = screenHalfHeight * mainCamera.aspect;

		// Choose a random prefab from the array
		GameObject selectedMeteorPrefab = meteorPrefabs[Random.Range(0, meteorPrefabs.Length)];

		
		// Get the enemy renderer component
		var meteorRenderer = selectedMeteorPrefab.GetComponent<Renderer>();

		if (meteorRenderer is null)
		{
			Debug.LogError("Meteor prefab must have a Renderer component for accurate width calculation.");
			return;
		}

		// Calculate the maximum spawn area considering the meteor size
		var bounds = meteorRenderer.bounds;
		var maxSpawnX = screenHalfWidth - bounds.extents.x;
		var maxSpawnY = screenHalfHeight - bounds.extents.y;

		// Randomly choose whether to spawn from top, bottom, or right side
		var spawnSide = Random.Range(0, 3);

		float spawnX, spawnY;
		var meteorRotation = Quaternion.identity; // Initialize with an appropriate default value
		var meteorVelocity = Vector3.zero;

		if (spawnSide == 0)
		{
			spawnX = Random.Range(0, maxSpawnX);
			spawnY = maxSpawnY;

			meteorRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

			var randomXVelocity = Random.Range(-8f, -2f); // Random speed between -10 and -2 for the X component
			var randomYVelocity = Random.Range(-8f, -2f); // Random speed between -10 and -2 for the Y component
			meteorVelocity = new Vector2(randomXVelocity, randomYVelocity);


		}
		else if (spawnSide == 1)
		{
			spawnX = Random.Range(0, maxSpawnX);
			spawnY = -maxSpawnY;

			meteorRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

			var randomXVelocity = Random.Range(-2f, -8f);
			var randomYVelocity = Random.Range(2f, 8f);
			meteorVelocity = new Vector2(randomXVelocity, randomYVelocity);
		}
		else
		{
			spawnX = maxSpawnX;
			spawnY = Random.Range(-maxSpawnY, maxSpawnY);

			meteorRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

			var randomXVelocity = Random.Range(-8f, -2f); // Random speed between -10 and -2 for the X component
			var randomYVelocity = Random.Range(-8f, 8f); // Random speed between -10 and -2 for the Y component
			meteorVelocity = new Vector2(randomXVelocity, randomYVelocity);
		}

		var spawnPosition = new Vector3(mainCamera.transform.position.x + spawnX, spawnY, 0);
		//Debug.Log("Degree is:" + meteorRotation);
		
		// Instantiate meteor
		var meteor = Instantiate(selectedMeteorPrefab, spawnPosition, meteorRotation);
		
		// Set a random scale for the meteor
		var randomScale = Random.Range(0.36f, 1.27f);
		meteor.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
		
		// Set velocity for the meteor to move towards the left
		var meteorRigidbody = meteor.GetComponent<Rigidbody2D>(); // Assuming you have a Rigidbody2D on the meteorPrefab TODO: what if you don't --> tryGetcomponent
		if (meteorRigidbody is not null)
		{
			// Adjust the velocity based on the random angle
			meteorRigidbody.velocity = meteorVelocity;
		}
		else
		{
			Debug.LogError("Meteor prefab must have a Rigidbody2D component for velocity control.");
		}
	}
}
