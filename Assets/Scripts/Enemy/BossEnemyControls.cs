using System.Collections;
using UnityEngine;

public class BossEnemyControls : MonoBehaviour
{
    [SerializeField] private float speed = 6f; // Adjust the speed as needed
    private Vector2 objectSize;
    [SerializeField] private GameObject normalBulletPrefab;
    [SerializeField] private GameObject specialBulletPrefab;
    [SerializeField] private float normalBulletInterval = 2f; // Time between normal bullet spawns
    [SerializeField] private float specialBulletInterval = 5f; // Time between special bullet spawns
    [SerializeField] private float specialBulletDuration = 3f; // Time duration of special bullet firing
    [SerializeField] private Transform firePointN;
    [SerializeField] private Transform firePointS1;
    [SerializeField] private Transform firePointS2;
    [SerializeField] private Animator fireballAnimatorS1;
    [SerializeField] private Animator fireballAnimatorS2;
    [SerializeField] private float bulletNSpeed = 5f; // Adjust the speed as needed
    [SerializeField] private float bulletSSpeed = 10f; // Adjust the speed as needed
    private GameObject player;
    private EnemyBossHealth enemyBossHealth;

    [SerializeField] private float initialLaserDelay = 10f;

    private int currentLife;


    private Animator animator;
    private string explosionTriggerName = "DestroyEnemy"; // The name of the trigger parameter
    private bool isExploding = false; // To keep track of the explosion state// Reference to the Animator component


    //private Transform targetL2;
    private void Start()
    {
        // Try to find the EnemyBossHealth component in the Canvas
        enemyBossHealth = FindObjectOfType<EnemyBossHealth>();

        if (enemyBossHealth == null)
        {
            Debug.LogError("EnemyBossHealth component not found in the scene.");
        }


        player = PlayerManager.instance.player;
        objectSize = GetObjectBoundsSize();
        InvokeRepeating("SpawnNormalBullet", 0f, normalBulletInterval);
        // InvokeRepeating("SpawnSpecialBullet", 0f, specialBulletInterval + specialBulletDuration + 1.5f);
        StartCoroutine(SpecialBulletRoutine());
    }


    private void Awake()
    {
        // Get the Animator component from this GameObject or one of its children
        animator = GetComponent<Animator>();
    }

    private IEnumerator SpecialBulletRoutine()
    {
        yield return new WaitForSeconds(initialLaserDelay);
        while (true)
        {
            // Prepare and fire lasers at both fire points simultaneously
            StartCoroutine(PrepareAndFireLaser(firePointS1, fireballAnimatorS1));
            StartCoroutine(PrepareAndFireLaser(firePointS2, fireballAnimatorS2));

            // Wait for specialBulletInterval + laserDuration for the next cycle
            yield return new WaitForSeconds(specialBulletInterval + specialBulletDuration + 1.5f);
        }
    }


    private void Update()
    {
        MoveBoss();
    }

    private Vector2 GetObjectBoundsSize()
    {
        var collider = GetComponent<Collider2D>(); //TODO: tryGetComponent!
        if (collider is not null)
        {
            var bounds = collider.bounds;
            return new Vector2(bounds.extents.x, bounds.extents.y);
        }
        else
        {
            Debug.LogWarning("No collider found on the object. Cannot determine bounds size.");
            return Vector2.zero;
        }
    }

    private void MoveBoss()
    {
        // Get the current position of the boss
        var currentPosition = transform.position;

        // Calculate the new position based on the y-axis movement
        var newY = Mathf.PingPong(Time.time * speed, Camera.main.orthographicSize * 2) - Camera.main.orthographicSize;

        // Calculate the screen boundaries
        var minY = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y + objectSize.y;
        var maxY = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y - objectSize.y;

        // Clamp the newY position within the screen limits
        newY = Mathf.Clamp(newY, minY, maxY);

        // Update the boss position
        transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);
    }
    // Set the target player for the enemy to follow

    private void MoveBoss2()
    {
        var target = player.transform;
        if (target is not null)
        {
            var currentPosition = transform.position;
            // Calculate the direction towards the target player
            Vector2 direction = (target.position - transform.position).normalized;

            direction.x = 0;

            // Move towards the target player
            transform.Translate(direction * (speed * Time.deltaTime), Space.World);

            // Ensure the enemy is facing the direction of movement
            //var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
            //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }


    private void SpawnNormalBullet()
    {
        var directionToPlayer = getPlayerDirection();
        var bulletN = Instantiate(normalBulletPrefab, firePointN.position, firePointN.rotation);
        var rigidbody = bulletN.GetComponent<Rigidbody2D>();

        if (rigidbody)
        {
            rigidbody.velocity = directionToPlayer * bulletNSpeed;
            faceTowardsPlayer(directionToPlayer, rigidbody);
        }
    }


    private Vector3 getPlayerDirection()
    {
        var playerPosition = player.transform.position;

        var directionToPlayer = (playerPosition - transform.position).normalized;
        return directionToPlayer;
    }

    private void faceTowardsPlayer(Vector3 directionToPlayer, Rigidbody2D bulletRigid)
    {
        var angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        bulletRigid.rotation = angle - 90f; // Adjust the angle if necessary
    }


    private IEnumerator PrepareAndFireLaser(Transform firePoint, Animator fireballAnimator)
    {
        // Start the preparation animation
        fireballAnimator.SetBool("IsPreparing", true);

        // Wait for the animation to reach its end
        yield return new WaitUntil(() =>
            fireballAnimator.GetCurrentAnimatorStateInfo(0).IsName("FireballPreparation") &&
            fireballAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);


        // Fire the laser after the preparation is done
        FireLaser(firePoint);
        // Turn off preparation
        fireballAnimator.SetBool("IsPreparing", false);

        // Wait for the laser to be done
        yield return new WaitForSeconds(specialBulletDuration);

        // Add delay for next firing sequence to start
        yield return new WaitForSeconds(specialBulletInterval);
    }


    private void FireLaser(Transform firePoint)
    {
        GameObject laserInstance = Instantiate(specialBulletPrefab, firePoint.position, Quaternion.identity, firePoint);
        laserInstance.transform.localPosition = Vector3.zero;
        laserInstance.transform.localRotation = Quaternion.identity;


        BossL1LaserController laserController = laserInstance.GetComponent<BossL1LaserController>();
        if (laserController != null)
        {
            laserController.ActivateLaser(specialBulletDuration);
        }
        else
        {
            Debug.LogError("LaserController script not found on the instantiated laser prefab.");
        }
    }


    // private void FireLaser(Transform firePoint)
    // {
    //     // GameObject laserInstance = Instantiate(specialBulletPrefab, firePoint.position, Quaternion.identity);
    //     GameObject laserInstance = Instantiate(specialBulletPrefab, firePoint.position, Quaternion.identity, firePoint);
    //
    //     // laserInstance.transform.SetParent(firePoint, false);
    //     laserInstance.transform.localPosition = Vector3.zero;
    //     laserInstance.transform.localRotation = Quaternion.identity;
    //
    //
    //     BossL1LaserController laserController = laserInstance.GetComponent<BossL1LaserController>();
    //     if (laserController != null)
    //     {
    //         laserController.ActivateLaser(firePoint, specialBulletDuration);
    //     }
    //     else
    //     {
    //         Debug.LogError("LaserController script not found on the instantiated laser prefab.");
    //     }
    // }
    // private IEnumerator WaitAndPlayExplosion(int damageAmount)
    // {
    //     // Play the explosion animation
    //     PlayExplosionAnimation();
    //
    //     // Wait for 5 seconds to allow the animation to finish
    //     yield return new WaitForSeconds(5);
    //
    //     // Apply the damage after waiting
    //     enemyBossHealth.TakeDamage(damageAmount, gameObject);
    //
    //     // Additional actions after damage is applied, such as checking if the boss is defeated
    //     if (enemyBossHealth.currentHealth <= 0)
    //     {
    //         // Actions to take when the boss is defeated, for example:
    //         Debug.Log("Boss defeated!");
    //         // Optionally destroy the boss object or deactivate it
    //         gameObject.SetActive(false);
    //         // Optionally load a new scene or trigger any other logic for boss defeat
    //     }
    // }


    private void OnTriggerEnter2D(Collider2D other)
    {
        currentLife = enemyBossHealth.currentHealth;
        Debug.Log("asdfasdfasdf" + LevelManager.Instance.isExploding);
        if (other.CompareTag("PlayerBullet") && !LevelManager.Instance.isExploding)
        {
            // Assuming bullets have a script or component that defines the damage they deal
            Bullet bullet = other.GetComponent<Bullet>();
            //Debug.Log(bullet);
            if (bullet != null)
            {
                int damageAmount = bullet.getDamagePoint(); // Adjust this based on your bullet script
                Debug.Log("Boss is collided with player bullet" + enemyBossHealth);

                // if (enemyBossHealth.currentHealth == 1)
                // {
                // PlayExplosionAnimation();
                // StartCoroutine(WaitAndDestroyBoss()); // Start the coroutine instead of destroying immediately
                // StartCoroutine(WaitAndPlayExplosion(damageAmount));
                // }
                // else
                // {
                enemyBossHealth.TakeDamage(damageAmount, gameObject);
                // }
                // PlayExplosionAnimation();
            }

            // Instantiate explosion effect
            // Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // if (enemyBossHealth.currentHealth <= 0)
            // {
            //     Debug.Log("Boss should be dead here");
            //
            //     PlayExplosionAnimation();
            //
            //     // Destroy(gameObject);
            //     // Handle boss defeat here
            //     // gameObject.SetActive(false); // Or Destroy(gameObject);
            //     //SceneManager.LoadScene("VictoryScene");
            // }
            // else
            // {
            //     // Handle boss hit but not defeated
            //     // Add any additional logic here if needed
            // }
        }
        else
        {
            Debug.Log("Player Collided with something other than PlayerBullet");
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