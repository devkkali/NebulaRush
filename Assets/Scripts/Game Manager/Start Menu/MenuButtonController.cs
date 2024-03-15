using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // Add this to use the new Input System

public class MenuButtonController : MonoBehaviour
{
    public int index;
    [SerializeField] bool keyDown;
    [SerializeField] int maxIndex;
    [SerializeField] TextMeshProUGUI scoreTextGameOver; // Assign your TextMeshPro UI element in the Inspector

    // References to AudioSources
    public AudioSource audioSource;
    public AudioSource backgroundMusicSource;
    public AudioSource buttonNavigationSoundSource;
    public AudioSource submitSoundSource;

    // Audio clips for background music and button navigation sound
    public AudioClip backgroundMusicClip;
    public AudioClip buttonNavigationSoundClip;
    public AudioClip submitSoundClip;

    // Reference to the Input Actions
    public InputActionReference navigate;
    public InputActionReference submit;
    

    private void OnEnable()
    {
        // Enable the Input Actions
        navigate.action.Enable();
        submit.action.Enable();
    }

    private void OnDisable()
    {
        // Disable the Input Actions when the object is disabled
        navigate.action.Disable();
        submit.action.Disable();
    }

    void Start()
    {
        // Get AudioSources components
        backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        buttonNavigationSoundSource = gameObject.AddComponent<AudioSource>();
        submitSoundSource = gameObject.AddComponent<AudioSource>();

        // Set up background music
        backgroundMusicSource.clip = backgroundMusicClip;
        backgroundMusicSource.loop = true;
        backgroundMusicSource.Play();
    }

    void Update()
    {
        // Use the Input Action to read the Navigate value
        var navigateValue = navigate.action.ReadValue<Vector2>();
        var submitValue = submit.action.triggered; // Check if the Submit action was triggered this frame

        if (navigateValue.y != 0)
        {
            if (!keyDown)
            {
                if (navigateValue.y < 0)
                {
                    if (index < maxIndex)
                    {
                        index++;
                    }
                    else
                    {
                        index = 0;
                    }
                }
                else if (navigateValue.y > 0)
                {
                    if (index > 0)
                    {
                        index--;
                    }
                    else
                    {
                        index = maxIndex;
                    }
                }

                keyDown = true;

                // Play button navigation sound
                buttonNavigationSoundSource.PlayOneShot(buttonNavigationSoundClip);
            }
        }
        else
        {
            keyDown = false;
        }

        // Use the Submit action's triggered state
        if (submitValue)
        {
            submitSoundSource.PlayOneShot(submitSoundClip);
            // Perform your submit action here, for example:
            Debug.Log("Submit action was triggered." + index);
            // You can call a function or perform an action here.
            if (index == 0)
            {
                // SceneManager.LoadScene("EasyLevelScene");

                // StartLevelTransition("EasyLevelScene");

                GameManager.Instance.StartLevelTransition("EasyLevelScene");
            }else if (index ==1)
            {
                Application.Quit();
            }
        }

        GameOverScore();
    }



    private void GameOverScore()
    {
        scoreTextGameOver.text = $"Score: {ScoreManager.Instance.GetCurrentScore()}";
    }
}