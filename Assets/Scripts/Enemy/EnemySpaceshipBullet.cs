using System.Collections;
using UnityEngine;

public class EnemySpaceshipBullet : MonoBehaviour
{
    public GameObject bulletPrefab; // Assign the enemy bullet prefab in the Inspector

    // public GameObject player; // Assign the player GameObject in the Inspector
    public float firingRate = 1f; // How often the enemy fires
    public float bulletSpeed = 5f; // Speed of the bullet
    public float bulletSpawnOffset = 1f; // Distance in front of the enemy where bullets spawn

    private float nextFireTime;
    private GameObject player;
    [SerializeField] private int scoreForDestroy = 7;

	private Animator animator;
	private string explosionTriggerName = "DestroyEnemy"; // The name of the trigger parameter
	private bool isExploding = false; // To keep track of the explosion state// Reference to the Animator component

	private void Start()
    {
        // Find the player in the scene and assign it
        // player = GameObject.FindGameObjectWithTag("Player"); //TODO: better: create a manager that you can access static. Then save the ship as SerializeField. This method you have used is inefficient!
        player = PlayerManager.instance.player;
        // Initialize the nextFireTime
        nextFireTime = Time.time + firingRate;
    }
	private void Awake()
	{
		// Get the Animator component from this GameObject or one of its children
		animator = GetComponent<Animator>();
	}

	private void Update()
    {
        // Check if it's time to fire
        if (Time.time >= nextFireTime && player && player.activeInHierarchy)
        {
            FireAtPlayer();
            nextFireTime = Time.time + firingRate; // Set the time for the next shot
        }
    }

    private void FireAtPlayer()
    {
        if (player && bulletPrefab && player.activeInHierarchy)
        {
            // Update the player position every time we fire
            var playerPosition = player.transform.position;

            // Calculate the direction from the enemy to the player
            var directionToPlayer = (playerPosition - transform.position).normalized;

            // Offset the spawn position in front of the enemy
            var spawnPosition = transform.position + directionToPlayer * bulletSpawnOffset;

            // Instantiate the bullet
            var bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            var rigidbody = bullet.GetComponent<Rigidbody2D>(); //TODO: rename variable and TryGetComponent!

            if (rigidbody)
            {
                // Set the bullet velocity towards the player
                rigidbody.velocity = directionToPlayer * bulletSpeed;
                
                // Rotate the bullet to face the player
                // float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
                // float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg-90f;
                // rigidbody.rotation = angle;

                // Debug log to show where the bullet is being instantiated
                // Debug.Log($"Bullet fired towards player at position: {playerPosition}");
                
                
                
                // Rotate the bullet to face towards the direction it's moving
                var angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
                rigidbody.rotation = angle - 90f; // Adjust the angle if necessary
                
                
            }
            else
            {
                Debug.LogError("Rigidbody not found on the bullet prefab!");
            }
        }
        else
        {
            if (!player)
            {
                Debug.LogError("Player object not assigned or not found!");
            }

            if (!bulletPrefab)
            {
                Debug.LogError("Bullet prefab not assigned!");
            }
        }
    }
    
    
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.CompareTag("PlayerBullet") || collision.CompareTag("Player"))
		{
			Debug.Log("Player Bullet collided somewhere");
			// Move the bullet to the collision point
			transform.position = collision.ClosestPoint(transform.position);
			Debug.Log("Player Bullet collided somewhere2");
			PlayExplosionAnimation();

			// ScoreManager.Instance.AddScore(5);
			ScoreManager.Instance.AddPoint(scoreForDestroy);

		}
	}



	// Call this function when you want to play the explosion animation
	private void PlayExplosionAnimation()
	{
		// Check if the bullet is already exploding to prevent multiple calls
		if (isExploding) return;

		Debug.Log("Player Bullet explosion");

		isExploding = true;
		animator.SetTrigger(explosionTriggerName);

		// Optionally: Disable the collider here
		var collider = GetComponent<Collider2D>(); //TODO: rename variable and TryGetComponent!
		if (collider != null)
		{
			collider.enabled = false;
		}

		// Disable the Rigidbody2D to stop any movement
		var rigidbody2D = GetComponent<Rigidbody2D>(); //TODO: rename variable and TryGetComponent!
		if (rigidbody2D != null)
		{
			rigidbody2D.velocity = Vector2.zero;
			rigidbody2D.isKinematic = true; // Prevents the Rigidbody from responding to physics
		}

		// Optional: Change layer or tag to prevent further collisions
		// gameObject.layer = LayerMask.NameToLayer("Ignore Collisions"); // Make sure the "Ignore Collisions" layer exists and is set to ignore other layers as needed

		// Wait for the animation to finish before destroying the bullet
		StartCoroutine(WaitForDestructionAnimation());
	}





	// A coroutine to wait for the animation to finish
	private IEnumerator WaitForDestructionAnimation()
	{
		// Wait for the Animator to transition to the explosion state
		yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyExplosionAnimation"));

		// Wait for the explosion animation to reach its end
		yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

		// Now you can safely destroy the bullet GameObject
		Destroy(gameObject);
	}


}