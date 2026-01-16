using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private Image[] fadeImages; // multiple panels to box player

    [Header("Settings")]
    [SerializeField] private float loadDuration = 3f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    Coroutine loadRoutine;

    void OnEnable()
    {
        StartLoading(loadDuration);
    }

    /* ---------- PUBLIC API ---------- */

    public void StartLoading(float duration)
    {
        if (loadRoutine != null)
            StopCoroutine(loadRoutine);

        loadRoutine = StartCoroutine(LoadRoutine(duration));
    }

    /* ---------- CORE ---------- */

    IEnumerator LoadRoutine(float duration)
    {
        loadingSlider.value = 0f;

        // Make all fade images fully dark
        SetFadeAlpha(1f);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            loadingSlider.value = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        loadingSlider.value = 1f;

        // Fade back to clear
        yield return StartCoroutine(FadeOut(fadeOutDuration));

        gameObject.SetActive(false);
        loadRoutine = null;
    }

    IEnumerator FadeOut(float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float alpha = 1f - (t / duration);
            SetFadeAlpha(alpha);
            yield return null;
        }

        SetFadeAlpha(0f);
    }

    void SetFadeAlpha(float alpha)
    {
        if (fadeImages == null) return;

        foreach (var img in fadeImages)
        {
            if (img == null) continue;

            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }
    }
}
