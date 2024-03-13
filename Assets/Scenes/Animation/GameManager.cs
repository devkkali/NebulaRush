using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject menuCanvas;
    public GameObject lunchCanvas;
    public Animator launchAnimationObject;


    // New references for the loading screen
    public GameObject loadingScreen;
    public Slider loadingProgressBar;
    public TextMeshProUGUI loadingProgressText;

    private void Awake()
    {
        // Singleton pattern to ensure only one GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist this object between scene reloads
        }
        else
        {
            Destroy(gameObject); // Ensure there are no duplicates
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Call your load preferences method here
        Debug.Log("&&&&&&&&&&&&&&&&&&&&" + scene.name);
        if (scene.name != "HomeMenuScene" && scene.name != "GameOverScene")
        {
            ScoreManager.Instance.LoadScorefromPrefs();
        }
    }

    public void StartLevelTransition(string levelName)
    {
        Debug.Log("Boss is killed and go to scene" + levelName);
        if (levelName == "Level1" || levelName == "EasyLevelScene")
        {
            Debug.Log("Boss is killed and go to scene Easy" + levelName);

            // For Level 1, use the launch animation
            menuCanvas.SetActive(false);
            lunchCanvas.SetActive(true);
            StartCoroutine(LoadLevelAfterAnimation(levelName));
        }
        else
        {
            Debug.Log("Boss is killed and go to scene other level:" + levelName);

            // For other levels, directly show the loading screen
            StartCoroutine(LoadLevelWithLoadingScreen(levelName));
        }
    }

    private IEnumerator LoadLevelAfterAnimation(string levelName)
    {
        Debug.Log("loading animation");
        // Trigger the start of the lunch animation
        launchAnimationObject.SetTrigger("StartLunchAnimation");

        // Wait for the Animator to start playing
        yield return null;

        // Now wait until the current state finishes playing
        while (launchAnimationObject.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        Debug.Log("waiting animation finished");
        // Optionally add a slight delay here if needed to allow for transition effects
        // yield return new WaitForSeconds(0.5f);
        Debug.Log("waiting o.5 finished");

        // Load the next level
        SceneManager.LoadScene(levelName);
    }


    private IEnumerator LoadLevelWithLoadingScreen(string levelName)
    {
        
        Debug.Log($"[GameManager] Starting load of {levelName} with loading screen.");

        loadingScreen.SetActive(true); // Show the loading screen

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName);
        asyncLoad.allowSceneActivation = false; // Prevent the scene from being activated immediately after loading
        
        
        // Introduce a minimum display time for the loading screen
        float minimumDisplayTime = 3.0f; // Minimum 3 seconds display
        float loadStartTime = Time.time;
        
        
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // Calculate progress
            loadingProgressBar.value = progress;
            loadingProgressText.text = $"Loading... {(int)(progress * 100)}%";

            if (asyncLoad.progress >= 0.9f && !asyncLoad.allowSceneActivation)
            {
                Debug.Log("[GameManager] Scene load nearly complete, ready to activate scene.");

                // Consider calling PreloadGameData here, if data needs to be preloaded before the scene activates
                // PreloadGameData();

                // Ensure the loading screen is displayed for a minimum amount of time
                if (Time.time - loadStartTime >= minimumDisplayTime)
                {
                    Debug.Log("[GameManager] Minimum display time reached, activating scene.");
                    asyncLoad.allowSceneActivation = true; // Allow the scene to activate
                }
            }

            yield return null;
        }

        Debug.Log("[GameManager] Scene loaded and activated." + ScoreManager.Instance.getBulletLevel());
        loadingScreen.SetActive(false); // Hide the loading screen after loading is complete
    }


    // Implement this method to preload data before the scene becomes active
    private void PreloadGameData()
    {
        // Example: Load score and bullet level from PlayerPrefs or another data source
        Debug.Log("Boss is killed and start Preloading is called");

        ScoreManager.Instance.LoadScorefromPrefs();
        // Implement other data loading as needed
        Debug.Log("Boss is killed and start Preloading is called" + ScoreManager.Instance.GetCurrentScore());
        Debug.Log("Boss is killed and start Preloading is called" + ScoreManager.Instance.getBulletLevel());
    }
}