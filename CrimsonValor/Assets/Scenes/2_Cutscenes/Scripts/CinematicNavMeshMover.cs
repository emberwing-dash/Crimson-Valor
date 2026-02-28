using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[System.Serializable]
public class MoveStep
{
    public Transform target;
    public bool run;
    public float arriveDistance;
}

public class CinematicNavMeshMover : MonoBehaviour
{
    [Header("Core")]
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Path")]
    public MoveStep[] path;

    [Header("Animator Parameters")]
    public string walkBool = "IsWalking";
    public string runBool = "IsRunning";
    public string readyBool = "isReady";   // lowercase

    [Header("Ready Settings")]
    public float readyDuration = 2f;

    private Coroutine moveRoutine;

    void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        if (animator != null)
            StartCoroutine(ReadyThenMove());
    }

    IEnumerator ReadyThenMove()
    {
        // 1️⃣ READY STATE
        animator.SetBool(readyBool, true);
        agent.isStopped = true;

        yield return new WaitForSeconds(readyDuration);

        animator.SetBool(readyBool, false);

        // 2️⃣ NOW movement is allowed
        if (path != null && path.Length > 0)
            StartMovement();
    }

    // -------------------- MOVEMENT --------------------

    IEnumerator FollowPath()
    {
        foreach (MoveStep step in path)
        {
            if (step.target == null)
                continue;

            agent.isStopped = false;
            agent.stoppingDistance = step.arriveDistance;
            agent.SetDestination(step.target.position);

            // ✅ WALK/RUN ONLY AFTER READY
            SetMoveAnim(step.run);

            while (agent.pathPending ||
                   agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            StopMoveAnim();
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            yield return null;
        }

        ResetMovementState();
        moveRoutine = null;
    }

    // -------------------- ANIMATION HELPERS --------------------

    void SetMoveAnim(bool run)
    {
        animator.SetBool(walkBool, !run);
        animator.SetBool(runBool, run);
    }

    void StopMoveAnim()
    {
        animator.SetBool(walkBool, false);
        animator.SetBool(runBool, false);
    }

    void ResetMovementState()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        animator.SetBool(walkBool, false);
        animator.SetBool(runBool, false);
    }

    // -------------------- PUBLIC --------------------

    public void StartMovement()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(FollowPath());
    }

    public void StopMovement()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        ResetMovementState();
        moveRoutine = null;
    }
}
