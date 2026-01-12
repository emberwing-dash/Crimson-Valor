using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;

    [Tooltip("Assign your XR Origin / Player root in the Inspector")]
    public Transform playerRoot;

    [Header("Enemy Settings")]
    public float speed = 1.5f;
    public float stoppingDistance = 1.5f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Set agent parameters
        agent.speed = speed;
        agent.stoppingDistance = stoppingDistance;
        agent.acceleration = 8f;
        agent.angularSpeed = 200f;
        agent.updateRotation = false; // We'll handle rotation manually
        agent.updatePosition = true;
    }

    void Update()
    {
        if (!playerRoot || !agent.isOnNavMesh) return;

        // Project player position onto NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(playerRoot.position, out hit, 3f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        // Rotate enemy to face player smoothly
        Vector3 direction = playerRoot.position - transform.position;
        direction.y = 0f; // keep rotation horizontal
        if (direction.sqrMagnitude > 0.01f) // avoid zero vector errors
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}
