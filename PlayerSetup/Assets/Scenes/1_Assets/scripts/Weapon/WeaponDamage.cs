using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public int damage;

    private void Awake()
    {
        // Assign damage based on tag
        switch (gameObject.tag)
        {
            case "Sword":
                damage = 3;
                break;

            case "Axe":
                damage = 4;
                break;

            case "GreatSword":
                damage = 6;
                break;

            default:
                damage = 1;
                break;
        }
    }
}
