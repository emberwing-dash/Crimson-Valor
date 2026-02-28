using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 25;
    public int currentHealth;

    [Header("UI")]
    public Slider healthSlider;

    [Header("Who Can Damage Me")]
    public string damageFromTag; // PlayerAttack OR EnemyAttack

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(damageFromTag))
            return;

        WeaponDamage weapon = other.GetComponent<WeaponDamage>();
        if (weapon != null)
        {
            TakeDamage(weapon.damage);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died");
        gameObject.SetActive(false);
    }
}
