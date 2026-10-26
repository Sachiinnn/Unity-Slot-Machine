
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image symbolImage;

    [Header("Symbols - Keep the same order on every reel")]
    [SerializeField] private Sprite[] symbols;

    public int SymbolCount => symbols == null ? 0 : symbols.Length;

    public bool IsSpinning { get; private set; }

    private Coroutine spinCoroutine;

    public void SpinTo(int targetIndex, float duration)
    {
        if (spinCoroutine != null)
        {
            StopCoroutine(spinCoroutine);
        }
         

        spinCoroutine = StartCoroutine(SpinRoutine(targetIndex, duration));
    }

    private IEnumerator SpinRoutine(int targetIndex, float duration)
    {
        if (symbolImage == null || SymbolCount == 0)
        {
            Debug.LogError($"{name}: Reel references are missing.");
            IsSpinning = false;
            spinCoroutine = null;
            yield break;
        }

        targetIndex = Mathf.Clamp(targetIndex, 0, SymbolCount - 1);
        duration = Mathf.Max(0.1f, duration);

        IsSpinning = true;

        float elapsed = 0f;
        float nextChange = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            nextChange -= Time.deltaTime;

            if (nextChange <= 0f)
            {
                int randomIndex = Random.Range(0, SymbolCount);
                symbolImage.sprite = symbols[randomIndex];

                
                float progress = Mathf.Clamp01(elapsed / duration);   // Increase the interval near the end to simulate slowing.
                float slowFactor = Mathf.Lerp(1f, 4f, progress);

                nextChange = 0.06f * slowFactor;
            }

            yield return null;
        }

       
        symbolImage.sprite = symbols[targetIndex];     // Always show the result selected by the game manager.

        IsSpinning = false;
        spinCoroutine = null;
    }
}

