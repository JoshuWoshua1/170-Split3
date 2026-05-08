using UnityEngine;

// Game manager, keep very simple
// only put things here that must be reset on game end, and must
// be accessable by multiple scripts, or between scenes.
// Most varialbes here should be reset upon game end, since this script
// is persistant between scenes, and will not be reset upon reloading the scene.

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    // Reset these on game end ----
    [SerializeField] private int playerScore = 0;
    [SerializeField] private int highScore = 0;

    // ----------------------------

    public void AddScore(int points)
    {
        playerScore += points;
        if (playerScore > highScore)
        {
            highScore = playerScore;
        }
    }

    public int RetrieveHighScore()
    {
        Debug.Log("Current High Score: " + highScore);
        return highScore;
    }

    public void ResetGame()
    {
        playerScore = 0;
    }
}
