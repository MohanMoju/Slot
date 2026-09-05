using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReelController : MonoBehaviour
{
    [Header("Reel References")]
    [SerializeField] private RectTransform symbolContainer;

    // Assign your UI Image components here (e.g., Top, Middle, Bottom symbols)
    [SerializeField] private Image[] symbolImages;

    [Header("Symbol Sprites")]
    // Assign your actual symbol graphics here in the same order as your SlotSymbol Enum
    [SerializeField] private Sprite[] symbolSprites;

    [Header("Spin Settings")]
    [SerializeField] private float spinSpeed = 800f;
    [SerializeField] private float spinDuration = 2f;
    [SerializeField] private float symbolHeight = 120f;

    private Vector2 startPosition;

    public bool IsSpinning { get; private set; }

    private void Start()
    {
        if (symbolContainer == null)
        {
            Debug.LogError($"{gameObject.name}: Symbol Container is NOT assigned!");
            return;
        }

        startPosition = symbolContainer.anchoredPosition;
    }

    public IEnumerator Spin(int targetSymbol)
    {
        if (IsSpinning || symbolContainer == null)
            yield break;

        IsSpinning = true;
        float elapsed = 0f;

        // 1. FAST SPIN WITH LOOPING ILLUSION
        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;

            // Move the container down
            symbolContainer.anchoredPosition += Vector2.down * spinSpeed * Time.deltaTime;

            // If the container moves down by exactly one symbol's height, snap it back up 
            // and randomize the symbols to create the illusion of infinite new symbols falling.
            if (symbolContainer.anchoredPosition.y < startPosition.y - symbolHeight)
            {
                symbolContainer.anchoredPosition += Vector2.up * symbolHeight;
                RandomizeSymbols();
            }

            yield return null;
        }

        // 2. SET THE FINAL RESULT
        SetFinalSymbol(targetSymbol);

        // 3. SMOOTH SNAP TO CENTER
        float returnDuration = 0.4f;
        float returnElapsed = 0f;
        Vector2 currentPosition = symbolContainer.anchoredPosition;

        while (returnElapsed < returnDuration)
        {
            returnElapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, returnElapsed / returnDuration);
            symbolContainer.anchoredPosition = Vector2.Lerp(currentPosition, startPosition, t);

            yield return null;
        }

        symbolContainer.anchoredPosition = startPosition;
        IsSpinning = false;
    }

    // Changes all images to a random sprite to create a blur/motion effect
    private void RandomizeSymbols()
    {
        if (symbolImages == null || symbolSprites == null || symbolSprites.Length == 0) return;

        foreach (Image img in symbolImages)
        {
            if (img != null)
            {
                img.sprite = symbolSprites[Random.Range(0, symbolSprites.Length)];
            }
        }
    }

    // Ensures the target symbol stops in the center (assuming index 1 is the middle image)
    private void SetFinalSymbol(int targetSymbol)
    {
        if (symbolImages == null || symbolSprites == null || symbolSprites.Length == 0) return;
        if (targetSymbol < 0 || targetSymbol >= symbolSprites.Length) return;

        RandomizeSymbols(); // Randomize top and bottom padding symbols

        // Assuming your setup has 3 images: Top (0), Center (1), Bottom (2)
        // We want the target result to show up in the center image.
        int targetIndex = symbolImages.Length > 1 ? 1 : 0;

        symbolImages[targetIndex].sprite = symbolSprites[targetSymbol];
    }
}