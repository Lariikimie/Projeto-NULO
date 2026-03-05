using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game manager using the Singleton pattern.
/// Coordinates game lifecycle events (start, pause, restart, quit).
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referências")]
    [SerializeField] private GameStateManager gameStateManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (gameStateManager == null)
            gameStateManager = GameStateManager.Instance;

        if (gameStateManager == null)
            Debug.LogWarning("[GameManager] GameStateManager não encontrado. Arraste-o no Inspector.");
    }

    /// <summary>Pauses the game.</summary>
    public void PauseGame()
    {
        if (gameStateManager != null)
            gameStateManager.SetPaused(true);
    }

    /// <summary>Resumes the game from a paused state.</summary>
    public void ResumeGame()
    {
        if (gameStateManager != null)
            gameStateManager.SetPaused(false);
    }

    /// <summary>Restarts the current scene.</summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>Quits the application.</summary>
    public void QuitGame()
    {
        Debug.Log("[GameManager] Saindo do jogo.");
        Application.Quit();
    }
}