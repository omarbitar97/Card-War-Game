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
    //public async UniTask SetCardImage(Image targetImage, string imageUrl)
    //{
    //    using (var request = UnityWebRequestTexture.GetTexture(imageUrl))
    //    {
    //        await request.SendWebRequest();

    //        if (request.result == UnityWebRequest.Result.Success)
    //        {
    //            Texture2D tex = DownloadHandlerTexture.GetContent(request);
    //            targetImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    //        }
    //        else
    //        {
    //            Debug.LogError("Failed to load card image: " + imageUrl);
    //        }
    //    }
    //}

    // Improved Version to flip the card image instead of instantly swapping
    //public async UniTask SetCardImage(Image target, string imageUrl)
    //{
    //    using (var request = UnityWebRequestTexture.GetTexture(imageUrl))
    //    {
    //        await request.SendWebRequest();

    //        if (request.result != UnityWebRequest.Result.Success)
    //        {
    //            Debug.LogError("Failed to load card image: " + imageUrl);
    //            return;
    //        }

    //        var tex = DownloadHandlerTexture.GetContent(request);
    //        var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

    //        // Flip the card image instead of instantly swapping
    //        var flipper = target.GetComponent<CardFlipper>();
    //        if (flipper != null)
    //            await flipper.FlipTo(sprite);
    //        else
    //            target.sprite = sprite; // fallback
    //    }
    //}

    public async UniTask SetCardImage(Image target, string imageRef)
    {
        // Check if it's a URL or local code
        if (imageRef.StartsWith("http"))
        {
            using (var request = UnityWebRequestTexture.GetTexture(imageRef))
            {
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Failed to load card image from web: " + imageRef);
                    return;
                }

                var tex = DownloadHandlerTexture.GetContent(request);
                var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                await ApplySpriteWithFlip(target, sprite);
            }
        }
        else
        {
            // Local card asset from Resources
            var sprite = Resources.Load<Sprite>($"Cards/{imageRef}");
            if (sprite == null)
            {
                Debug.LogError($"Missing local sprite: Cards/{imageRef}");
                return;
            }

            await ApplySpriteWithFlip(target, sprite);
        }
    }

    private async UniTask ApplySpriteWithFlip(Image target, Sprite sprite)
    {
        var flipper = target.GetComponent<CardFlipper>();
        if (flipper != null)
            await flipper.FlipTo(sprite);
        else
            target.sprite = sprite;
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
