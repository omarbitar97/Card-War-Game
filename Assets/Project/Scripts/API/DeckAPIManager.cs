using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Cysharp.Threading.Tasks;
using static CardModel;

/// <summary>
/// Handles communication with the Deck of Cards API for card operations.
/// Implements ICardService to allow switching between API and local implementations.
/// </summary>
///
public class DeckAPIManager : MonoBehaviour, ICardService
{
    public static DeckAPIManager Instance;

    // Base URL for the Deck of Cards API 
    private string baseURL = "https://deckofcardsapi.com/api/deck";
    private string deckId; // Stores the current active deck ID from the API

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Initializes a new shuffled deck via API using coroutines.
    public void InitializeDeck(Action onComplete)
    {
        StartCoroutine(GetNewDeckCoroutine(onComplete));
    }

    // Async version of deck initialization 
    // True if initialization succeeded, false otherwise
    public async UniTask<bool> InitializeDeckAsync()
    {
        try
        {
            string url = $"{baseURL}/new/shuffle/?deck_count=1";
            using (var request = UnityWebRequest.Get(url))
            {
                request.timeout = 10; // Set reasonable timeout to avoid hanging

                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Deck initialization failed: {request.error}");
                    return false;
                }

                // Parse the response and extract the deck ID
                var json = JsonUtility.FromJson<DeckResponse>(request.downloadHandler.text);
                if (json == null || string.IsNullOrEmpty(json.deck_id))
                {
                    // API sometimes returns success but with invalid data
                    Debug.LogError("Invalid deck response from API.");
                    return false;
                }

                deckId = json.deck_id;
                return true;
            }
        }
        catch (Exception ex)
        {
            // Network errors or serialization issues will land here
            Debug.LogError("Exception in InitializeDeckAsync: " + ex.Message);
            return false;
        }
    }

    // Draws a card using coroutines
    public void DrawCard(Action<CardModel> onCardDrawn)
    {
        StartCoroutine(DrawCardCoroutine(onCardDrawn));
    }

    // Legacy coroutine implementation for deck initialization
   private IEnumerator GetNewDeckCoroutine(Action onComplete)
    {
        string url = $"{baseURL}/new/shuffle/?deck_count=1";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Deck initialization failed: " + request.error);
            yield break;
        }

        var json = JsonUtility.FromJson<DeckResponse>(request.downloadHandler.text);
        deckId = json.deck_id;
        onComplete?.Invoke();
    }

    // Legacy coroutine implementation for card drawing
    private IEnumerator DrawCardCoroutine(Action<CardModel> onCardDrawn)
    {
        if (string.IsNullOrEmpty(deckId))
        {
            Debug.LogError("Deck ID is not set. Call InitializeDeck first.");
            yield break;
        }

        string url = $"{baseURL}/{deckId}/draw/?count=1";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Card draw failed: " + request.error);
            yield break;
        }

        var json = JsonUtility.FromJson<DrawResponse>(request.downloadHandler.text);
        var cardData = json.cards[0];
        var card = new CardModel(cardData.code, cardData.image, cardData.value, cardData.suit);
        onCardDrawn?.Invoke(card);
    }

    // Async version of deck initialization
    public async UniTask<CardModel> DrawCardAsync()
    {
        if (string.IsNullOrEmpty(deckId))
        {
            Debug.LogError("Deck ID is not set. Call InitializeDeckAsync first.");
            return null;
        }

        try
        {
            string url = $"{baseURL}/{deckId}/draw/?count=1";
            using (var request = UnityWebRequest.Get(url))
            {
                request.timeout = 10;

                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Card draw failed: " + request.error);
                    return null;
                }

                var json = JsonUtility.FromJson<DrawResponse>(request.downloadHandler.text);
                if (json == null || json.cards == null || json.cards.Length == 0)
                {
                    // This could happen if the deck runs out of cards
                    Debug.LogError("Invalid card draw response.");
                    return null;
                }

                var cardData = json.cards[0];
                return new CardModel(cardData.code, cardData.image, cardData.value, cardData.suit);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception during card draw: " + ex.Message);
            return null;
        }
    }

    // Data classes for JSON deserialization
    [Serializable]
    private class DeckResponse { public string deck_id; }

    [Serializable]
    private class DrawResponse
    {
        public CardData[] cards;
    }

    [Serializable]
    private class CardData
    {
        public string code;
        public string image;
        public string value;
        public string suit;
    }
}
