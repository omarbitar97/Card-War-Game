using UnityEngine;
using Cysharp.Threading.Tasks;
using static CardModel;


/// <summary>
/// Manages the gameplay flow for each round of the card game.
/// Handles card drawing, comparison, and result determination.
/// </summary>
public class RoundManager : MonoBehaviour
{
    // Reference to the card service component , allows for dependency injection
    [SerializeField] private MonoBehaviour cardServiceSource;
    private ICardService cardService;

    private bool roundInProgress = false;

    private void Awake()
    {
        // Get the card service implementation from the assigned component
        cardService = cardServiceSource as ICardService;
        if (cardService == null)
        {
            Debug.LogError("Card service must implement ICardService.");
        }
    }

    public async void StartRound()
    {
        // Early exit if the game is over or a round is already in progress
        if (GameManager.Instance.IsGameOver() || roundInProgress) return;

        roundInProgress = true;
        UIManager.Instance.SetDrawButtonEnabled(false);

        await HandleRoundAsync();

        roundInProgress = false;
    }

    // Draws cards for both players, displays them, and determines the winner.
    // Async version of Round Handling
    private async UniTask HandleRoundAsync()
    {
        //var playerCard = await cardService.DrawCardAsync();
        var playerCard = await GameManager.Instance.CardService.DrawCardAsync();

        if (playerCard == null) return;

        await UIManager.Instance.SetCardImage(UIManager.Instance.playerCardImage, playerCard.ImageUrl);

        int latency = UnityEngine.Random.Range(600, 1500);
        UIManager.Instance.ShowStatus($"Waiting for opponent... ({latency}ms)");
        await UniTask.Delay(latency);
        UIManager.Instance.HideStatus();

        //var botCard = await cardService.DrawCardAsync();
        var botCard = await GameManager.Instance.CardService.DrawCardAsync();
        if (botCard == null) return;

        await UIManager.Instance.SetCardImage(UIManager.Instance.botCardImage, botCard.ImageUrl);

        string result = (playerCard.Value == botCard.Value)
            ? "It's a Tie!"
            : playerCard.Value > botCard.Value
                ? "Player Wins the Round!"
                : "Bot Wins the Round!";

        // Update scores if not a tie
        if (playerCard.Value != botCard.Value)
        {
            GameManager.Instance.AddScore(playerCard.Value > botCard.Value);
        }

        // Show results and re-enable draw button if game isn't over
        if (!GameManager.Instance.IsGameOver())
        {
            UIManager.Instance.ShowResult(result);
            UIManager.Instance.SetDrawButtonEnabled(true);
        }
    }
}
