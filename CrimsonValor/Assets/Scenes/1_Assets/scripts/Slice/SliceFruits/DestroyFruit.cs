using UnityEngine;

public class DestroyFruit : MonoBehaviour
{
    [Header("Destruction Settings")]
    [SerializeField] private float destroyAfterSeconds = 5f; // time until object is destroyed

    private void Start()
    {
        // Safety check
        if (destroyAfterSeconds <= 0f)
        {
            destroyAfterSeconds = 1f; // minimum 1 second
        }

        // Destroy this GameObject after the interval
        Destroy(gameObject, destroyAfterSeconds);
    }
}
