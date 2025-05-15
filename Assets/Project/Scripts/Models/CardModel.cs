using Cysharp.Threading.Tasks;
using UnityEngine;


/// <summary>
/// Represents a playing card with its associated data and functionality.
/// Handles card value parsing and comparison for the game logic.
/// </summary>
[System.Serializable]
public class CardModel
{
    public string Code;      // e.g. "AS"
    public string ImageUrl;  // for UI
    public int Value;        // 2-14 (2–10, J=11, Q=12, K=13, A=14)
    public string Suit;      // Hearts, Spades, etc.

    // Creates a new card model with the specified attributes.
    public CardModel(string code, string imageUrl, string valueStr, string suit)
    {
        Code = code;
        ImageUrl = imageUrl;
        Suit = suit;
        Value = ParseCardValue(valueStr);
    }

    // Converts string card values to numeric values for comparison.
    private int ParseCardValue(string val)
    {
        return val switch
        {
            "ACE" => 14,
            "KING" => 13,
            "QUEEN" => 12,
            "JACK" => 11,
            _ => int.TryParse(val, out int num) ? num : 0
        };
    }

    // Interface for card services to standardize deck operations.
    // Allows for different implementations (API-based, local, mock for testing).
    public interface ICardService
    {
        UniTask<bool> InitializeDeckAsync();
        UniTask<CardModel> DrawCardAsync();
    }
}
