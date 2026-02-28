using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class HealthManager : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Canvas DeadCanva;
    private int currentHealth;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;

    [Header("Death Settings")]
    [SerializeField] private float loadDelay = 10f;
    [SerializeField] private string menuSceneName = "Menu";

    private AudioSource deathAudio;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        DeadCanva.gameObject.SetActive(false);

        // Get AudioSource from DeadCanvas
        deathAudio = DeadCanva.GetComponent<AudioSource>();

        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void Die()
    {
        isDead = true;

        Debug.Log("Player Dead");

        DeadCanva.gameObject.SetActive(true);

        if (deathAudio != null)
        {
            deathAudio.Play();
        }

        StartCoroutine(LoadMenuAfterDelay());
    }

    private IEnumerator LoadMenuAfterDelay()
    {
        yield return new WaitForSeconds(loadDelay);
        SceneManager.LoadScene(menuSceneName);
    }
}
