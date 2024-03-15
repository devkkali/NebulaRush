using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpaceshipControls : MonoBehaviour
{
    [SerializeField] private Transform cubeTransform;

    [SerializeField] private int movingSpeed = 10;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private GameObject shieldObject;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefabL1;
    [SerializeField] private GameObject bulletPrefabL2;
    [SerializeField] private GameObject bulletPrefabL3;
    [SerializeField] private float bullet1Speed = 5f;
    [SerializeField] private float bullet2Speed = 5f;
    [SerializeField] private float bullet3Speed = 5f;
    // [SerializeField] private int PlayerLavel = 1;

    [SerializeField] private GameObject muzzleFlashObject;
    [SerializeField] private float muzzleDisplayTime = 0.1f; // Time in seconds to display the muzzle flash

    [SerializeField] private float invincibilityTime = 2.0f;
    private bool isInvincible = false;
    public float invincibilityDuration = 4f; // Default duration for invincibility
    private float remainingInvincibilityTime = 0f;
    private Coroutine invincibilityCoroutine;
    private bool canReceiveNewShield = true;
    private bool canReceiveNewBullet = true;
    private bool canReceiveNewHealth = true;
    private float powerPickupCooldown = 0.5f;
    private bool pickedUpShieldDuringInvincibility = false;
    [SerializeField] private float bounceForce = 2f;


    public easyLevelAudio audioManager; // Reference to the easyLevelAudio script


    public HealthBar healthBar;


    private List<SpriteRenderer> spriteRenderers; // Store all sprite renderers
    // [SerializeField] private GameObject shieldGameObject; // Assign in the inspector


    public InputActionReference move;
    public InputActionReference fire;

    private Vector2 moveDirection;
    private Camera mainCamera;
    private Vector2 screenBounds;
    private Vector2 objectSize;

    private Animator animator;
    private Vector3 lastPosition;


    //private ScoreManager scoreManager;


    private void Start()
    {
        Debug.Log("##########################################################################start ");
        mainCamera = Camera.main;
        objectSize = GetObjectBoundsSize();
        // currentLives = maxLives;
        lastPosition = transform.position;
        // Find the easyLevelAudio in the scene if it's not already assigned
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<easyLevelAudio>();
        }

        // PlayerLavel=ScoreManager.Instance.getBulletLevel("level change");
        Debug.Log("Bullet level at Start" + ScoreManager.Instance.getBulletLevel());


        // Find and cache the PlayerScore script on the player object
        /** playerScore = FindObjectOfType<PlayerScore>();
        if (playerScore == null)
        {
            UnityEngine.Debug.LogError("PlayerScore script not found on the player object!");
        }
        **/
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>(true));

        // UpdateHealthFill();
    }

    private Vector2 GetObjectBoundsSize()
    {
        var collider = GetComponent<Collider2D>();
        if (collider != null)
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

    private void Update()
    {
        // Debug.Log("remainingInvincibilityTime:" + remainingInvincibilityTime);
        moveDirection = move.action.ReadValue<Vector2>();
        if (fire.action.triggered)
        {
            Fire();
            BulletAudio();
        }
    }

    private void FixedUpdate()
    {
        // Debug.Log("moving direction" + moveDirection);
        // Move the cubea
        rigidbody.velocity = new Vector2(moveDirection.x * movingSpeed, moveDirection.y * movingSpeed);

        // Calculate screen bounds dynamically considering aspect ratio
        var screenRatio = (float)Screen.width / Screen.height;
        var orthoSize = mainCamera.orthographicSize;
        screenBounds = new Vector2(orthoSize * screenRatio, orthoSize);

        // Clamp the cube's position to keep it within the screen bounds
        var clampedPosition = transform.position;
        clampedPosition.x =
            Mathf.Clamp(clampedPosition.x, -screenBounds.x + objectSize.x, screenBounds.x - objectSize.x);
        clampedPosition.y =
            Mathf.Clamp(clampedPosition.y, -screenBounds.y + objectSize.y, screenBounds.y - objectSize.y);
        transform.position = clampedPosition;


        // Pass the moveDirection directly to the Animator to drive the Blend Tree
        animator.SetFloat("MoveX", moveDirection.x);
        animator.SetFloat("MoveY", moveDirection.y);

        // Logging movement for debugging
        // if (moveDirection == Vector2.zero)
        // {
        //     Debug.Log("Idle");
        // }
        // else
        // {
        //     if (moveDirection.x > 0)
        //     {
        //         Debug.Log("Moving Right");
        //     }
        //     else if (moveDirection.x < 0)
        //     {
        //         Debug.Log("Moving Left");
        //     }
        //     if (moveDirection.y > 0)
        //     {
        //         Debug.Log("Moving Up");
        //     }
        //     else if (moveDirection.y < 0)
        //     {
        //         Debug.Log("Moving Down");
        //     }
        // }

        // float distanceTraveled = Vector3.Distance(transform.position, lastPosition);
        // ScoreManager.Instance.AddDistanceScore(distanceTraveled);
        // lastPosition = transform.position;
    }

    public void Fire()
    {
        // var bullet = new GameObject();
        GameObject bullet;

        if (ScoreManager.Instance.getBulletLevel() == 1)
        {
            bullet = Instantiate(bulletPrefabL1, firePoint.position, firePoint.rotation);
            SetBulletVelocity(bullet, bullet1Speed);
        }
        else if (ScoreManager.Instance.getBulletLevel() == 2)
        {
            bullet = Instantiate(bulletPrefabL2, firePoint.position, firePoint.rotation);
            SetBulletVelocity(bullet, bullet2Speed);
        }
        else if (ScoreManager.Instance.getBulletLevel() == 3)
        {
            // bullet = Instantiate(bulletPrefabL3, firePoint.position, firePoint.rotation);

            // Instantiate the middle bullet going straight
            var bulletMiddle = Instantiate(bulletPrefabL3, firePoint.position, firePoint.rotation);
            SetBulletVelocity(bulletMiddle, bullet3Speed);

            // Calculate rotations for the angled bullets
            Quaternion rotationLeft = Quaternion.Euler(0, 0, firePoint.rotation.eulerAngles.z + 10);
            Quaternion rotationRight = Quaternion.Euler(0, 0, firePoint.rotation.eulerAngles.z - 10);

            // Instantiate the left bullet with a rotation of 10 degrees
            var bulletLeft = Instantiate(bulletPrefabL3, firePoint.position, rotationLeft);
            SetBulletVelocity(bulletLeft, bullet3Speed);

            // Instantiate the right bullet with a rotation of -10 degrees
            var bulletRight = Instantiate(bulletPrefabL3, firePoint.position, rotationRight);
            SetBulletVelocity(bulletRight, bullet3Speed);
        }

        StartCoroutine(ShowMuzzleFlash());
    }

    private void SetBulletVelocity(GameObject bullet, float bulletSpeed)
    {
        if (bullet.TryGetComponent<Rigidbody2D>(out var rigidBody))
        {
            rigidBody.velocity = bullet.transform.up * bulletSpeed;
        }
        else
        {
            Debug.LogWarning("Rigidbody2D component not found on the bullet prefab.");
        }
    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger (2D physics only)
    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "PlayerBullet":
                // Ignore cllision with player's own bullet
                break;
            case "PowerHealth":
                // Increase Life
                Destroy(other.gameObject);
                GetLife();
                break;
            case "PowerShield":
                // GetShield
                Destroy(other.gameObject);
                GetShield();
                break;
            case "PowerBullet":
                //GetBullet
                Destroy(other.gameObject);
                GetBullet();
                break;
            case "Meteors":
                if (!isInvincible)
                {
                    HandleHarmfulCollision();
                }
                else
                {
                    Debug.Log("Else condition");
                    // Make the meteor bounce back
                    Rigidbody2D meteorRigidbody = other.GetComponent<Rigidbody2D>();
                    if (meteorRigidbody != null)
                    {
                        Debug.Log("Else have rigid condition");
                        // Calculate the direction to bounce the meteor away from the player
                        Vector2 bounceDirection = other.transform.position - transform.position;
                        bounceDirection.Normalize();
                        // Apply the force
                        meteorRigidbody.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
                    }
                }

                break;
            default:
                if (!isInvincible)
                {
                    HandleHarmfulCollision();
                    Destroy(other.gameObject);
                }

                break;
        }
    }


    private void HandleHarmfulCollision()
    {
        healthBar.LoseLife();
        ReducePlayerLevel();

        // Instantiate explosion effect
        // Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        if (healthBar.currentLives <= 0)
        {
            // Handle player death here
            gameObject.SetActive(false); // Or Destroy(gameObject);

            //SceneManager.LoadScene("GameOverScene");
            LevelManager.Instance.PlayerDefeated();
        }
        else
        {
            // Handle player hit but not dead, such as respawning
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        // Calculate the left boundary position based on the camera's view
        var leftBoundary = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        var verticalCenter = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height / 2f, 0)).y;

        // Get the player's sprite width to prevent spawning half off-screen
        var playerSpriteWidth = objectSize.x;
        if (spriteRenderers.Count > 0)
        {
            playerSpriteWidth = spriteRenderers[0].bounds.size.x / 2;
        }

        // Set the player's position to the left boundary (plus half sprite width) and vertically centered
        var respawnPosition = new Vector3(leftBoundary + playerSpriteWidth, verticalCenter, 0);
        rigidbody.position = respawnPosition; // Using Rigidbody2D's position for physics consistency

        // Begin the invincibility routine
        StartCoroutine(InvincibilityRoutine());
    }


    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        // spaceshipSpriteRenderer.enabled = false; // Start with the spaceship invisible

        // How often the sprite should flicker during invincibility
        var flickerInterval = 0.1f; //TODO: move to SeralizeField property if value must be changed later
        var elapsed = 0f;

        while (elapsed < invincibilityTime)
        {
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled; // Toggle visibility
            }

            yield return new WaitForSeconds(flickerInterval);
            elapsed += flickerInterval;
        }

        // Ensure all renderers are visible after invincibility ends
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.enabled = true;
        }

        isInvincible = false;

        // Check if a shield was picked up during invincibility
        if (pickedUpShieldDuringInvincibility)
        {
            ActivateShield();
            pickedUpShieldDuringInvincibility = false; // Reset the flag
        }
    }


    private void GetLife()
    {
        if (canReceiveNewHealth)
        {
            canReceiveNewHealth = false;
            StartCoroutine(HealthPickupCooldownRoutine());
            //   Debug.Log("current life is:" + healthBar.currentLives);
            healthBar.GainLife();
            //  Debug.Log("current life is2:" + healthBar.currentLives);
        }
    }

    private void GetBullet()
    {
        if (canReceiveNewBullet)
        {
            canReceiveNewBullet = false;
            StartCoroutine(BulletPickupCooldownRoutine());
            IncreasePlayerLevel();
        }
    }

    // Call this function when player picks up a shield power-up
    public void GetShield()
    {
        if (canReceiveNewShield)
        {
            remainingInvincibilityTime += invincibilityDuration;
            canReceiveNewShield = false;
            StartCoroutine(ShieldPickupCooldownRoutine());
            if (isInvincible)
            {
                // Set flag that a shield was picked up during invincibility
                pickedUpShieldDuringInvincibility = true;
            }
            else
            {
                ActivateShield();
            }
        }
    }

    private void ActivateShield()
    {
        isInvincible = true;
        shieldObject.SetActive(true);
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }

        invincibilityCoroutine = StartCoroutine(InvincibilityCountdown());
    }

    private IEnumerator ShieldPickupCooldownRoutine()
    {
        yield return new WaitForSeconds(powerPickupCooldown);
        canReceiveNewShield = true; // Reset flag after cooldown
    }

    private IEnumerator HealthPickupCooldownRoutine()
    {
        yield return new WaitForSeconds(powerPickupCooldown);
        canReceiveNewHealth = true; // Reset flag after cooldown
    }

    private IEnumerator BulletPickupCooldownRoutine()
    {
        yield return new WaitForSeconds(powerPickupCooldown);
        canReceiveNewBullet = true; // Reset flag after cooldown
    }

    private void ReducePlayerLevel()
    {
        ScoreManager.Instance.reduceBulletLevel();
        // PlayerLavel = Mathf.Max(PlayerLavel - 1, 1); // Ensure player level does not go below 1
    }

    private void IncreasePlayerLevel()
    {
        ScoreManager.Instance.increaseBulletLevel();
        //       PlayerLavel = Mathf.Min(PlayerLavel + 1, 3); // Ensure player level does not exceed max level
        // ScoreManager.Instance.ChangeBulletLevel(PlayerLavel);
        // Debug.Log("Bullet level at increase" + PlayerLavel);
    }

    private IEnumerator InvincibilityCountdown()
    {
        while (remainingInvincibilityTime > 0f)
        {
            yield return new WaitForSeconds(1f);
            remainingInvincibilityTime -= 1f;
        }

        // After the loop, ensure isInvincible is set to false and cleanup
        isInvincible = false;
        shieldObject.SetActive(false);
        remainingInvincibilityTime = 0f; // Reset to ensure clean state
        // Additional cleanup or state reset as needed
        pickedUpShieldDuringInvincibility = false; // Also reset this flag here just in case
    }

    private IEnumerator ShowMuzzleFlash()
    {
        muzzleFlashObject.SetActive(true); // Show the muzzle flash
        yield return new WaitForSeconds(muzzleDisplayTime); // Wait for the specified duration
        muzzleFlashObject.SetActive(false); // Hide the muzzle flash
    }

    public void BulletAudio()
    {
        // Play bullet firing audio
        audioManager.PlayBulletFiringAudio();
    }
}