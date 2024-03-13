using UnityEngine;

public class easyLevelAudio : MonoBehaviour
{
    public AudioClip backgroundMusicClip; // Assign the background music clip in the Inspector
    public AudioClip bossSpawnAudioClip; // Assign the boss spawn audio clip in the Inspector
	public AudioClip bulletFiringAudioClip; // Assign the bullet firing audio clip in the Inspector
    
    private AudioSource audioSource;

    private void Start()
    {
        // Add an AudioSource component if not already present
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set up background music
        audioSource.clip = backgroundMusicClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlayBossSpawnAudio()
    {
        // Stop background music
        audioSource.Stop();
        // Play boss spawn audio
        audioSource.clip = bossSpawnAudioClip;
        audioSource.loop = true;
        audioSource.Play();
    }

	public void PlayBulletFiringAudio()
    {
        // Play bullet firing audio
        audioSource.PlayOneShot(bulletFiringAudioClip);
    }
}