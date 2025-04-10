using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; } = false;

    public GameObject pauseMenuUI;
    public bool isPaused = false;

    public StarterAssetsInputs starterAssetsInputs;

    private void Start()
    {
        starterAssetsInputs = FindAnyObjectByType<StarterAssetsInputs>();
    }

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
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        starterAssetsInputs.lookingLocked = false;
        Time.timeScale = 1f;
        isPaused = false;
        IsPaused = false;

        // Re-lock the cursor after resuming
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PauseGame()
    {
        starterAssetsInputs.lookingLocked = true;
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }

        Time.timeScale = 0f;
        isPaused = true;
        IsPaused = true;

        // Unlock the cursor when paused
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReplayGame()
    {

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        IsPaused = false;
        starterAssetsInputs.lookingLocked = false;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is quitting...");
    }
}
