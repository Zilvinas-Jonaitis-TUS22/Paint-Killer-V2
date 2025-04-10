using UnityEngine;
using UnityEngine.SceneManagement; // For scene management

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign your pause menu UI canvas in the inspector
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ResumeGame()
    {
        // Null check for pauseMenuUI
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        // Null check for pauseMenuUI
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        Time.timeScale = 0f;
        isPaused = true;
    }

    // Function to restart the game (Replay button)
    public void ReplayGame()
    {
        // Reset the time scale to normal in case the game was paused
        Time.timeScale = 1f;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Optional: Call this to quit the game
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is quitting...");
    }
}
