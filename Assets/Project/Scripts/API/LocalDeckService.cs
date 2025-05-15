using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static CardModel;

/// <summary>
/// Manages the game fallback to a local deck implementation if no internet connection.
/// </summary>
public class LocalDeckService : MonoBehaviour, ICardService
{
    private List<CardModel> deck;

    public UniTask<bool> InitializeDeckAsync()
    {
        deck = GenerateFullDeck();
        Shuffle(deck);
        return UniTask.FromResult(true);
    }

    public UniTask<CardModel> DrawCardAsync()
    {
        if (deck == null || deck.Count == 0)
        {
            Debug.LogWarning("Local deck is empty.");
            return UniTask.FromResult<CardModel>(null);
        }

        var card = deck[0];
        deck.RemoveAt(0);
        return UniTask.FromResult(card);
    }

    private List<CardModel> GenerateFullDeck()
    {
        var suits = new[] { "HEARTS", "DIAMONDS", "CLUBS", "SPADES" };
        var values = new[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "JACK", "QUEEN", "KING", "ACE" };

        var list = new List<CardModel>();

        foreach (var suit in suits)
        {
            foreach (var value in values)
            {
                string code = GenerateCardCode(value, suit); // e.g., "AS"
                list.Add(new CardModel(code, code, value, suit)); // use code for image key too
            }
        }

        return list;
    }

    private string GenerateCardCode(string value, string suit)
    {
        string shortVal = value switch
        {
            "10" => "0", 
            "ACE" => "A",
            "KING" => "K",
            "QUEEN" => "Q",
            "JACK" => "J",
            _ => value
        };

        char suitChar = suit[0]; // H, D, C, S
        return shortVal + suitChar;
    }
    private void Shuffle(List<CardModel> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
