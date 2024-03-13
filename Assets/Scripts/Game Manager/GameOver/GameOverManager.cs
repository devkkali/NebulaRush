using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void Retry()
    {
        // Reload the game scene
        SceneManager.LoadScene("EasyLevelScene");
    }

    public void QuitToMainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("HomeScene");
    }
}