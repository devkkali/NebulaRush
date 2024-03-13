using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public GameObject bossPrefab;
    public GameObject bossHealth; // Assign this in the Inspector
    public float levelDistanceThreshold; // Distance before the boss appears
    private bool bossSpawned = false;

    [SerializeField] private Animator screenFlickerAnimator;

    // private int GameLevel = 0;
    private bool isFlickering = false;

    private easyLevelAudio audioManager; // Reference to easyLevelAudio

    private Camera mainCamera;

    [SerializeField] private int level = 1;


    // private Animator animator;
    private string explosionTriggerName = "DestroyEnemy"; // The name of the trigger parameter
    public bool isExploding = false; // To keep track of the explosion state// Reference to the Animator component


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        mainCamera = Camera.main; // Cache the main camera
        audioManager = FindObjectOfType<easyLevelAudio>();
        if (audioManager == null)
        {
            Debug.LogError("BackgroundAudioManager not found in the scene.");
        }

        // Load cumulative score when the game starts
        if (level == 1)
        {
            ScoreManager.Instance.ResetScore();
        }

        else
        {
            ScoreManager.Instance.LoadScorefromPrefs();
        }
    }

    void Update()
    {
        if (!bossSpawned && PlayerHasReachedDistanceThreshold() && level != 3)
        {
            // SpawnBoss();
            StartCoroutine(BossEntrySequence());

            bossSpawned = true;
            if (audioManager != null)
            {
                audioManager.PlayBossSpawnAudio(); // Call PlayBossSpawnAudio in easyLevelAudio
            }
        }
    }

    private bool PlayerHasReachedDistanceThreshold()
    {
        // Access the total distance from the ScoreManager
        float totalDistance =
            ScoreManager.Instance.GetTotalDistance(); // This method needs to be implemented in your ScoreManager

        // Check if the total distance is greater than or equal to the threshold
        //Debug.Log("Distance travelled: "+totalDistance);
        return totalDistance >= levelDistanceThreshold;
    }


    private IEnumerator BossEntrySequence()
    {
        // Start the screen flickering effect
        // Animator screenAnimator = screenFlickerAnimator/* Get your Animator reference */;
        // screenAnimator.SetTrigger("StartFlicker");
        // Instantiate the boss off-screen here and animate its entry
        var enemyBossRotation = Quaternion.Euler(0, 0, 90); // Adjust the Euler angles as needed for your prefab
        // Slowly move the boss onto the screen
        bossHealth.SetActive(true);
        GameObject boss = Instantiate(bossPrefab, GetOffScreenPosition(), enemyBossRotation);
        Coroutine bossEntry = StartCoroutine(AnimateBossEntry(boss));

        // Pause the game, but keep animations and coroutines that use real-time updating
        Time.timeScale = 0;

        // Wait for the boss to finish entering
        yield return bossEntry; // This will still complete because we're using real-time in the coroutine

        // Stop the screen flickering effect
        // screenAnimator.SetTrigger("StopFlicker");

        // Resume the game
        Time.timeScale = 1;

        // Continue with any other setup needed after boss has entered
    }

    // Here's how you can get an off-screen position to the right of the camera
    private Vector3 GetOffScreenPosition()
    {
        float screenHalfHeight = Camera.main.orthographicSize;
        float screenHalfWidth = screenHalfHeight * Camera.main.aspect;
        return new Vector3(Camera.main.transform.position.x + screenHalfWidth, 0, 0);
    }

    // Animate boss entering from the side of the screen to its starting position
    private IEnumerator AnimateBossEntry(GameObject boss)
    {
        var enemyBossRenderer = bossPrefab.GetComponent<Renderer>();


        var screenHalfHeight = mainCamera.orthographicSize;
        var screenHalfWidth = screenHalfHeight * mainCamera.aspect;
        // Calculate the maximum spawn area considering the enemy size
        var bounds = enemyBossRenderer.bounds;
        var maxSpawnX = screenHalfWidth - bounds.extents.x;
        var maxSpawnY = screenHalfHeight - bounds.extents.y;

        // Set spawn position on the extreme right y-axis border
        var spawnX = maxSpawnX;


        // Calculate spawn position with an offset from the center of the screen
        var spawnPosition = new Vector3(mainCamera.transform.position.x + spawnX, 0, 0);


        Vector3 startPosition = GetOffScreenPosition();
        Vector3 endPosition = spawnPosition; // Replace with the actual boss position on screen
        float duration = 3f; // Duration of the animation


        // Start flickering effect
        // Start flickering effect
        screenFlickerAnimator.SetBool("IsFlickering", true);


        // Animate the boss entry
        float startTime = Time.unscaledTime; // Using unscaled time if the game is paused
        while (Time.unscaledTime - startTime < duration)
        {
            float progress = (Time.unscaledTime - startTime) / duration;
            boss.transform.position = Vector3.Lerp(startPosition, endPosition, progress);

            // You could also add some scaling or other effects here if you want

            yield return null;
        }

        boss.transform.position = endPosition;
        // Wait a little bit more if you want to ensure the flickering continues for a while after the boss has stopped moving
        yield return new WaitForSecondsRealtime(0.5f); // Adjust as necessary
        // Stop flickering effect
        screenFlickerAnimator.SetBool("IsFlickering", false);
    }


    private void SpawnBoss()
    {
        var enemyBossRenderer = bossPrefab.GetComponent<Renderer>();

        // Stop the background scroll
        BGScroll bgScroll = FindObjectOfType<BGScroll>();
        if (bgScroll != null)
        {
            bgScroll.enabled = false;
        }

        // Instantiate the boss
        var enemyBossRotation = Quaternion.Euler(0, 0, 90); // Adjust the Euler angles as needed for your prefab

        var screenHalfHeight = mainCamera.orthographicSize;
        var screenHalfWidth = screenHalfHeight * mainCamera.aspect;
        // Calculate the maximum spawn area considering the enemy size
        var bounds = enemyBossRenderer.bounds;
        var maxSpawnX = screenHalfWidth - bounds.extents.x;
        var maxSpawnY = screenHalfHeight - bounds.extents.y;

        // Set spawn position on the extreme right y-axis border
        var spawnX = maxSpawnX;


        // Calculate spawn position with an offset from the center of the screen
        var spawnPosition = new Vector3(mainCamera.transform.position.x + spawnX, 0, 0);
        Instantiate(bossPrefab, spawnPosition, enemyBossRotation);
        bossHealth.SetActive(true);
    }


    // private bool isExploding = false;

    private IEnumerator WaitAndPlayExplosion(GameObject boss)
    {
        Debug.Log("[WaitAndPlayExplosion] Coroutin is called.");
        boss.GetComponent<PolygonCollider2D>().enabled = false;
        // boss.SetActive(false);
        Debug.Log("collider off");
        if (!isExploding)
        {
            isExploding = true;
            Debug.Log("[WaitAndPlayExplosion] Coroutine started.");

            Animator animator = boss.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[WaitAndPlayExplosion] Missing animator component on boss.");
                yield break;
            }

            animator.SetTrigger(explosionTriggerName);
            Debug.Log("[WaitAndPlayExplosion] Triggering explosion animation.");

            // Wait for the explosion state to start playing.
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyExplosionAnimation"));
            
            boss.SetActive(false);
            yield return new WaitForSeconds(1);
            // Debug.Log("[WaitAndPlayExplosion] Triggering explosion animation1.");
            //
            // // Wait for the explosion state to finish playing.
            // yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
            // Debug.Log("[WaitAndPlayExplosion] Triggering explosion animation2.");
            //
            // // Allow some time for the explosion to settle before proceeding.
            // yield return new WaitForSeconds(1); // Adjust this delay to match your needs.
            // Debug.Log("[WaitAndPlayExplosion] Triggering explosion animation3.");

            // Now transition to the next level or perform other actions.
            // Save the current score to PlayerPrefs
            ScoreManager.Instance.SaveScore();

            if (level == 1)
            {
                GameManager.Instance.StartLevelTransition("Level2");
            }
            else if (level == 2)
            {
                GameManager.Instance.StartLevelTransition("Level3");
            }
            else
            {
                Debug.LogError("[WaitAndPlayExplosion] Undefined level index: " + level);
            }

            isExploding = false;
            Debug.Log("[WaitAndPlayExplosion] Coroutine finished.");

            // Optionally delay the destruction of the boss object to ensure the coroutine has time to perform transitions.

            Destroy(boss, 0.1f); // Adjust the delay if necessary to ensure it happens after the transition.
        }
        // boss.SetActive(false);

        EnemyBossHealth.ResetExplosion();
        Debug.Log("[WaitAndPlayExplosion] Coroutine already running. Exiting early to avoid duplicate execution.");
    }


    public void BossDefeated(GameObject boss)
    {
        Debug.Log("Boss is killed");
        Debug.Log("Boss is killed of livel" + level);

        if (EnemyBossHealth.IsExploding)
        {
            StartCoroutine(WaitAndPlayExplosion(boss));
        }


        // Start next level or show victory screen
    }


    // Call this function when you want to play the explosion animation
    // private void PlayExplosionAnimation(GameObject boss)
    // {
    //     // Check if the bullet is already exploding to prevent multiple calls
    //     if (isExploding) return;
    //
    //     //Debug.Log("Player Bullet explosion");
    //
    //     isExploding = true;
    //     animator.SetTrigger(explosionTriggerName);
    //
    //     // Optionally: Disable the collider here
    //     var collider = GetComponent<Collider2D>(); //TODO: rename variable and TryGetComponent!
    //     if (collider != null)
    //     {
    //         collider.enabled = false;
    //     }
    //
    //     // Disable the Rigidbody2D to stop any movement
    //     var rigidbody2D = GetComponent<Rigidbody2D>(); //TODO: rename variable and TryGetComponent!
    //     if (rigidbody2D != null)
    //     {
    //         rigidbody2D.velocity = Vector2.zero;
    //         rigidbody2D.isKinematic = true; // Prevents the Rigidbody from responding to physics
    //     }
    //
    //     // Optional: Change layer or tag to prevent further collisions
    //     // gameObject.layer = LayerMask.NameToLayer("Ignore Collisions"); // Make sure the "Ignore Collisions" layer exists and is set to ignore other layers as needed
    //
    //     // Wait for the animation to finish before destroying the bullet
    //     StartCoroutine(WaitForDestructionAnimation(boss));
    // }


    // A coroutine to wait for the animation to finish
    // private IEnumerator WaitForDestructionAnimation(GameObject boss)
    // {
    //     // Wait for the Animator to transition to the explosion state
    //     yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyExplosionAnimation"));
    //
    //     // Wait for the explosion animation to reach its end
    //     yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
    //
    //     // Now you can safely destroy the bullet GameObject
    //     Destroy(boss);
    // }
}