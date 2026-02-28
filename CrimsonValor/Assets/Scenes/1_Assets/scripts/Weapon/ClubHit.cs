using UnityEngine;

public class ClubHit : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 10;
    public string playerTag = "Player";
    public bool onlyDuringAttack = true;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Club TRIGGERED by: {other.name} (tag: {other.tag})");

        // Check for Player tag (direct or root)
        bool isPlayer = other.CompareTag(playerTag) || other.transform.root.CompareTag(playerTag);
        Debug.Log($"Is player? {isPlayer}");

        if (!isPlayer) return;

        // Optional: only damage during goblin attack animation
        bool attacking = !onlyDuringAttack || IsGoblinAttacking();
        Debug.Log($"Goblin attacking? {attacking}");

        if (!attacking) return;

        // Get HealthManager from the player (direct or root)
        HealthManager health = other.GetComponent<HealthManager>();
        if (!health)
        {
            health = other.transform.root.GetComponent<HealthManager>();
            Debug.Log($"Found HealthManager on root: {health != null}");
        }

        if (health)
        {
            Debug.Log($"DAMAGING player for {damage}");
            health.TakeDamage(damage);
        }
        else
        {
            Debug.LogError("No HealthManager found on player!");
        }
    }

    bool IsGoblinAttacking()
    {
        GoblinAI goblin = GetComponentInParent<GoblinAI>();
        if (!goblin || !goblin.animator)
        {
            Debug.Log("No goblin/animator found");
            return true;
        }

        bool atk1 = goblin.animator.GetBool(goblin.isAtk1);
        bool atk2 = goblin.animator.GetBool(goblin.isAtk2);
        Debug.Log($"Animator atk1={atk1}, atk2={atk2}");
        return atk1 || atk2;
    }
}
