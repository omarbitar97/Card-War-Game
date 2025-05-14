using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Collections;
/// <summary>
/// Central manager for all UI elements and interactions in the game.
/// Handles card display, score updates, status messages, and button states.
/// </summary>
public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    [Header("Card Images")]
    public Image playerCardImage;
    public Image botCardImage;

    [Header("Score Texts")]
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI botScoreText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI resultText;

    [Header("Buttons")]
    public Button drawButton;

    [SerializeField] private TextMeshProUGUI statusText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    //public void SetCardImage(Image targetImage, string imageUrl)
    //{
    //    StartCoroutine(LoadCardSprite(targetImage, imageUrl));
    //}

    // Handles loading and creating sprites from remote texture resources.
    // Sets a card image using a URL asynchronously.
    public async UniTask SetCardImage(Image targetImage, string imageUrl)
    {
        using (var request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D tex = DownloadHandlerTexture.GetContent(request);
                targetImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("Failed to load card image: " + imageUrl);
            }
        }
    }

    // Legacy coroutine implementation for loading card sprites.
    // Kept for reference and backward compatibility.
    private IEnumerator LoadCardSprite(Image img, string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
                img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("Failed to load image: " + url);
            }
        }
    }

    public void UpdateScore(int playerScore, int botScore, int round)
    {
        playerScoreText.text = "Player: " + playerScore;
        botScoreText.text = "Bot: " + botScore;
        roundText.text = "Round: " + round;
    }

    public void ShowResult(string result)
    {
        resultText.text = result;
    }

    public void SetDrawButtonEnabled(bool state)
    {
        if (drawButton != null)
            drawButton.interactable = state;
    }


    public void ShowStatus(string message)
    {
        if (statusText != null)
        {
            statusText.gameObject.SetActive(true);
            statusText.text = message;
        }
    }

    public void HideStatus()
    {
        if (statusText != null)
            statusText.gameObject.SetActive(false);
    }
}
