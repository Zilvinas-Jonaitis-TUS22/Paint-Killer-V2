using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; } = false;

    public GameObject pauseMenuUI;
    public GameObject deathScreenUI;
    public bool isPaused = false;

    public StarterAssetsInputs starterAssetsInputs;

    private void Start()
    {
        // Reset time scale at start to avoid staying paused after reload
        Time.timeScale = 1f;
        IsPaused = false;
        isPaused = false;

        starterAssetsInputs = FindAnyObjectByType<StarterAssetsInputs>();

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (deathScreenUI != null) deathScreenUI.SetActive(false);
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
            pauseMenuUI.SetActive(false);

        starterAssetsInputs.lookingLocked = false;
        Time.timeScale = 1f;
        isPaused = false;
        IsPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PauseGame()
    {
        starterAssetsInputs.lookingLocked = true;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
        IsPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReplayGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        isPaused = false;
        starterAssetsInputs.lookingLocked = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is quitting...");
    }

    // 🔥 Call this from the health script when the player dies
    public void ShowDeathScreen()
    {
        Time.timeScale = 1f;
        isPaused = true;
        IsPaused = true;

        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);
        }

        starterAssetsInputs.lookingLocked = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
