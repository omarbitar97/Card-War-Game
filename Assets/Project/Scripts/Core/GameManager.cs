using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Central game manager that controls the overall game state, including
/// scores, rounds, and game end conditions. Acts as the primary controller.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int playerScore = 0;
    public int botScore = 0;
    public int currentRound = 0;
    public int maxScore = 8; // First player to reach this score wins

    private bool gameEnded = false;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

    }

    public async void Start()
    {
        await InitializeGameAsync();
    }

    // Performs asynchronous game initialization tasks including
    public async UniTask InitializeGameAsync()
    {
        StartGame();

        var success = await DeckAPIManager.Instance.InitializeDeckAsync();
        if (!success)
        {
            UIManager.Instance.ShowResult("Failed to load deck. Check your internet.");
            return;
        }
    }

    // Resets the game state for a new game.
    public void StartGame()
    {
        playerScore = 0;
        botScore = 0;
        currentRound = 0;
        gameEnded = false;
        Debug.Log("Game Started");
    }

    // Updates scores based on round outcome and checks for game end condition.
    public void AddScore(bool playerWon)
    {
        if (playerWon) playerScore++;
        else botScore++;

        currentRound++;

        UIManager.Instance.UpdateScore(playerScore, botScore, currentRound);

        if (playerScore >= maxScore)
        {
            EndGame(true);
        }
        else if (botScore >= maxScore)
        {
            EndGame(false);
        }
    }

    // Handles the end of the game, displaying the final result.
    public void EndGame(bool playerWon)
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("Game Ended. " + (playerWon ? "Player Wins!" : "Bot Wins!"));

        UIManager.Instance.ShowResult(playerWon ? " YOU WIN!" : " BOT WINS!");
        UIManager.Instance.SetDrawButtonEnabled(false);
    }

    public bool IsGameOver()
    {
        return gameEnded;
    }
}
