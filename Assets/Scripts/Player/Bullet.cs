using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	private Animator animator;
	private const string ExplosionTriggerName = "explode"; // The name of the trigger parameter
	private bool isExploding = false; // To keep track of the explosion state
	public int damage = 1;
	private void Awake()
	{
		animator = GetComponent<Animator>(); //TODO: TryGetComponent!!!!
	}



	// Update is called once per frame
	void Update()
	{
		// Destroy the bullet if it goes out of the screen
		if (!GetComponent<Renderer>().isVisible)
		{
			Destroy(gameObject);
		}
	}


	// Call this function when you want to play the explosion animation
	private void PlayExplosionAnimation()
	{
		// Check if the bullet is already exploding to prevent multiple calls
		if (isExploding) return;

		//Debug.Log("Player Bullet explosion");

		isExploding = true;
		animator.SetTrigger(ExplosionTriggerName);

		// Optionally: Disable the collider here
		var bulletCollider = GetComponent<Collider2D>(); //TODO: TryGetComponent!!!!
		if (bulletCollider != null) //use is not null
		{
			bulletCollider.enabled = false;
		}

		// Disable the Rigidbody2D to stop any movement
		var bulletRigidBody2D = GetComponent<Rigidbody2D>(); //TODO: TryGetComponent!!!!
		if (bulletRigidBody2D != null) //use is not null
		{
			bulletRigidBody2D.velocity = Vector2.zero;
			bulletRigidBody2D.isKinematic = true; // Prevents the Rigidbody from responding to physics
		}

		// Optional: Change layer or tag to prevent further collisions
		// gameObject.layer = LayerMask.NameToLayer("Ignore Collisions"); // Make sure the "Ignore Collisions" layer exists and is set to ignore other layers as needed

		// Wait for the animation to finish before destroying the bullet
		StartCoroutine(WaitForAnimation());
	}

	// A coroutine to wait for the animation to finish
	private IEnumerator WaitForAnimation()
	{
		// Wait for the Animator to transition to the explosion state
		yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("BulletCollisionAnimation"));

		// Wait for the explosion animation to reach its end
		yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

		// Now you can safely destroy the bullet GameObject
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Debug.Log("collision by Player bullet");
		// Check if the bullet collided with an object tagged as "Enemy"
		// If the bullet is already exploding, don't do anything.
		if (isExploding)
		{
		//	Debug.Log("Bullet is already exploding.");
			return;
		}
		
		// If the collided object is an enemy or an enemy bullet, handle the collision.
		if (collision.CompareTag("Enemy") || collision.CompareTag("EnemyBullet") || collision.CompareTag("Meteors"))
		{
		//	Debug.Log("Player Bullet collided with enemy or enemy bullet.");
        
			// Move the bullet to the collision point and play explosion animation.
			transform.position = collision.ClosestPoint(transform.position);
			PlayExplosionAnimation();
		}
		else
		{
			// For any other collision, we're explicitly doing nothing, but you can add any additional logic here if needed.
			Debug.Log("Collision with non-target object.");
		}
	}



	public int getDamagePoint()
	{
		return damage;
	}
}
