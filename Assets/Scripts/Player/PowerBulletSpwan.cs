using UnityEngine;

public class PowerBulletSpwan : MonoBehaviour
{
	public GameObject powerBulletPrefab; // Assign this in the Inspector
								
	[SerializeField] private float spawnRate = 10f;

	private Camera mainCamera;
	private float nextSpawnTime;

	// Start is called before the first frame update
	private void Start()
	{
		mainCamera = Camera.main; // Cache the main camera
		nextSpawnTime = Time.time + spawnRate; // Initialize the next spawn time
	}

	// Update is called once per frame
	private void Update()
	{
		// Check if it's time to spawn a new enemy
		if (Time.time >= nextSpawnTime)
		{
			SpawnPowerBullet();
			nextSpawnTime = Time.time + spawnRate; // Set the time for the next spawn
		}

	}

	private void SpawnPowerBullet()
	{
		// Determine the vertical bounds of the camera view
		var screenHalfHeight = mainCamera.orthographicSize;

		// Determine the horizontal bounds of the camera view
		var screenHalfWidth = screenHalfHeight * mainCamera.aspect;

		// Get the enemy renderer component
		var powerbulletRenderer = powerBulletPrefab.GetComponent<Renderer>();

		if (powerbulletRenderer is null)
		{
			Debug.LogError("Power Bullet prefab must have a Renderer component for accurate width calculation.");
			return;
		}

		// Calculate the maximum spawn area considering the power bullet prefab size
		var bounds = powerbulletRenderer.bounds;
		var maxSpawnX = screenHalfWidth - bounds.extents.x;
		var maxSpawnY = screenHalfHeight - bounds.extents.y;

		// Randomly choose whether to spawn from top, bottom, or right side
		var spawnSide = Random.Range(0, 3);

		float spawnX, spawnY;

		spawnX = maxSpawnX;
		spawnY = Random.Range(-maxSpawnY, maxSpawnY);

		Vector3 powerBulletVelocity = new Vector2(-2f, 0);

		var spawnPosition = new Vector3(mainCamera.transform.position.x + spawnX, spawnY, 0);

		// Instantiate power bullet
		var powerBullet = Instantiate(powerBulletPrefab, spawnPosition, Quaternion.identity);

		// Set velocity for the power bullet to move towards the left
		var powerBulletRigidbody = powerBullet.GetComponent<Rigidbody2D>(); // Assuming you have a Rigidbody2D on the powerbulletPrefab //TODO: repating: tryGetcomponent
		if (powerBulletRigidbody is not null)
		{
			// Adjust the velocity based on the random angle
			powerBulletRigidbody.velocity = powerBulletVelocity;
		}
		else
		{
			Debug.LogError("Power Bullet prefab must have a Rigidbody2D component for velocity control.");
		}
		Destroy(powerBullet, 10f);
	}
}
