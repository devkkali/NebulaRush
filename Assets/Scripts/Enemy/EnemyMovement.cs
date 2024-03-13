using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Animator animator;
    private string explosionTriggerName = "DestroyEnemy"; // The name of the trigger parameter
    private bool isExploding = false; // To keep track of the explosion state// Reference to the Animator component

    private Transform target;
    private float speed = 3f;
    
    [SerializeField] private int scoreForDestroy = 5; 



    private void Awake()
    {
        // Get the Animator component from this GameObject or one of its children
        animator = GetComponent<Animator>();
    }


    // Set the target player for the enemy to follow
    public void SetTarget(Transform newTarget)
    {
        // UnityEngine.Debug.Log("from setTarget");

        target = newTarget;
    }

    void Update()
    {
        // Check if a target is set
        if (target is not null)
        {
            // Calculate the direction towards the target player
            Vector2 direction = (target.position - transform.position).normalized;

            // Move towards the target player
            transform.Translate(direction * (speed * Time.deltaTime), Space.World);

            // Ensure the enemy is facing the direction of movement
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }


    // OnTriggerEnter2D is called when the Collider2D other enters the trigger (2D physics only)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enemy Collided");
        // Check if the object collided with has the tag "PlayerBullet"
        if (collision.CompareTag("PlayerBullet") || collision.CompareTag("Player"))
        {
            //Debug.Log("Player Bullet collided somewhere");
            // Move the bullet to the collision point
            transform.position = collision.ClosestPoint(transform.position);
            //Debug.Log("Player Bullet collided somewhere2");
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

        //Debug.Log("Player Bullet explosion");

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