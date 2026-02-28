using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownTrigger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Canvas startCanvas;
    [SerializeField] private Canvas countdownCanvas;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Animator textAnimator;

    [Header("Target")]
    [SerializeField] private GameObject fruitSpawner; // keep it disabled initially
    [SerializeField] private AudioSource countdownAudio; // plays countdown
    [SerializeField] private AudioSource backgroundAudio; // new audio to turn off

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 1f;

    private bool triggered = false;

    private void Start()
    {
        if (startCanvas != null)
            startCanvas.gameObject.SetActive(true);

        if (countdownCanvas != null)
            countdownCanvas.gameObject.SetActive(false);

        if (fruitSpawner != null)
            fruitSpawner.SetActive(false);

        if (countdownAudio != null)
            countdownAudio.enabled = false; // countdown initially off

        if (backgroundAudio != null)
            backgroundAudio.enabled = true; // keep background audio on until player triggers
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // Hide start UI immediately
        if (startCanvas != null)
            startCanvas.gameObject.SetActive(false);

        // Show countdown UI
        if (countdownCanvas != null)
            countdownCanvas.gameObject.SetActive(true);

        // Play countdown audio
        if (countdownAudio != null)
        {
            countdownAudio.enabled = true;
            countdownAudio.Play();
        }

        // Turn OFF background audio
        if (backgroundAudio != null)
        {
            backgroundAudio.enabled = false;
            backgroundAudio.Stop();
        }

        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        string[] countdownValues = { "3", "2", "1", "GO!!" };

        foreach (string value in countdownValues)
        {
            countdownText.text = value;

            if (textAnimator != null)
                textAnimator.Play("go", 0, 0f); // make sure "go" exists in Animator

            yield return new WaitForSeconds(animationDuration);
        }

        // Countdown finished
        if (countdownCanvas != null)
            countdownCanvas.gameObject.SetActive(false);

        if (fruitSpawner != null)
            fruitSpawner.SetActive(true);
    }
}
