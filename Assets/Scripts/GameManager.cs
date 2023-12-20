using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Playing,
        Paused,
        GameOver
    }

    public GameState gameState = GameState.Playing;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }

    public void StartGame()
    {
        gameState = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        gameState = GameState.Paused;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        gameState = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void EndGame()
    {
        gameState = GameState.GameOver;
        Time.timeScale = 0f;
    }

    public void ResetScene()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

}
