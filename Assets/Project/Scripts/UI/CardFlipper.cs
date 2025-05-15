using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// Manages the cards flip with Coroutine-based animation 
/// </summary>
public class CardFlipper : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private float flipDuration = 0.2f; // Half-flip time (one side)

    private Vector3 originalScale;
    private bool isDestroyed = false;

    private void OnDestroy()
    {
        isDestroyed = true;
    }

    private void Awake()
    {
        if (cardImage == null)
            cardImage = GetComponent<Image>();

        originalScale = transform.localScale;
    }

    // Animates the card flip and swaps the image mid-flip.
    public async UniTask FlipTo(Sprite newSprite)
    {
        if (this == null || gameObject == null) return;

        await ScaleXTo(0, flipDuration);

        if (this == null || gameObject == null) return;

        cardImage.sprite = newSprite;

        await ScaleXTo(originalScale.x, flipDuration);
    }

    private async UniTask ScaleXTo(float target, float duration)
    {
        if (this == null || gameObject == null) return;

        float time = 0f;
        float start = transform.localScale.x;

        while (time < duration)
        {
            if (this == null || gameObject == null) return;

            float x = Mathf.Lerp(start, target, time / duration);
            transform.localScale = new Vector3(x, originalScale.y, originalScale.z);
            time += Time.deltaTime;
            await UniTask.Yield();
        }

        // One final check before setting
        if (this != null && gameObject != null)
            transform.localScale = new Vector3(target, originalScale.y, originalScale.z);
    }

    private async UniTask SafeDelay(float duration)
    {
        float elapsed = 0;
        while (elapsed < duration && !isDestroyed)
        {
            elapsed += Time.deltaTime;
            await UniTask.Yield();
        }
    }
}
