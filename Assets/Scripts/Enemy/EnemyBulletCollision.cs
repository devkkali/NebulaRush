using System.Collections;
using UnityEngine;

public class EnemyBulletCollision : MonoBehaviour
{    
    private Animator animator;
    private const string ExplosionTriggerName = "explodeEnemy"; // The name of the trigger parameter
    private bool isExploding = false; // To keep track of the explosion state

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    // Call this function when you want to play the explosion animation
    private void PlayExplosionAnimation()
    {
        // Check if the bullet is already exploding to prevent multiple calls
        if (isExploding) return;

        Debug.Log("Enemy Bullet explosion");

        isExploding = true;
        animator.SetTrigger(ExplosionTriggerName);

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
       // Debug.Log("collision by Player bullet"); 
        // Check if the bullet collided with an object tagged as "Enemy"
        if ((!collision.CompareTag("Player") && !collision.CompareTag("PlayerBullet")) || isExploding) return;
       // Debug.Log("Enemy Bullet collided somewhere");
        // Move the bullet to the collision point
        transform.position = collision.ClosestPoint(transform.position);
        PlayExplosionAnimation();
    }
    
    
}