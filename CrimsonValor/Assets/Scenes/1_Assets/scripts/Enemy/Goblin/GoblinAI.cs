using UnityEngine;
using UnityEngine.AI;

public class GoblinAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;
    public BoxCollider searchArea;
    public Transform eyes;

    [Header("Movement")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 3.5f;
    public float idleTime = 2f;
    public float attackDistance = 2.2f;

    [Header("Vision")]
    public float viewDistance = 18f;
    [Range(0, 180)] public float viewAngle = 120f;
    public int rayCount = 9;

    [Header("Combat")]
    public float attackCooldown = 1.3f;
    public int damage = 10;
    [HideInInspector] public bool hitPlayer;

    [Header("Animator Params")]
    public string isLooking = "isLooking";
    public string isWalk = "isWalk";
    public string isRun = "isRun";
    public string isAtk1 = "isAtk1";
    public string isAtk2 = "isAtk2";

    enum State { Patrol, Idle, Chase, Attack }
    State state = State.Patrol;

    Transform player;
    HealthManager playerHealth;

    Vector3 patrolTarget;
    float idleTimer;
    float nextAttackTime;

    bool canSeePlayer;
    bool attackInProgress;
    string currentAttack;

    /* ===================== SETUP ===================== */

    void Awake()
    {
        agent ??= GetComponent<NavMeshAgent>();
        animator ??= GetComponent<Animator>();

        GameObject p = GameObject.FindGameObjectWithTag("PlayerHealth");
        if (p)
        {
            player = p.transform;
            playerHealth = p.GetComponent<HealthManager>();
        }

        InitializePatrol();
    }

    public void InitializePatrol()
    {
        state = State.Patrol;
        attackInProgress = false;
        currentAttack = null;
        hitPlayer = false;

        ResetAnims();
        SetNewPatrolPoint();

        agent.isStopped = false;
        agent.ResetPath();
    }

    /* ===================== UPDATE ===================== */

    void Update()
    {
        UpdateVision();

        switch (state)
        {
            case State.Patrol: Patrol(); break;
            case State.Idle: Idle(); break;
            case State.Chase: Chase(); break;
            case State.Attack: Attack(); break;
        }
    }

    /* ===================== VISION ===================== */

    void UpdateVision()
    {
        canSeePlayer = false;
        if (!player) return;

        Vector3 origin = eyes ? eyes.position : transform.position + Vector3.up * 1.6f;
        Vector3 forward = transform.forward;

        float half = viewAngle * 0.5f;
        float step = viewAngle / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            float angle = -half + step * i;
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * forward;

            Debug.DrawRay(origin, dir * viewDistance, Color.red);

            if (Physics.Raycast(origin, dir, out RaycastHit hit, viewDistance))
            {
                if (hit.collider.CompareTag("PlayerHealth"))
                {
                    canSeePlayer = true;
                    return;
                }
            }
        }
    }

    /* ===================== PATROL ===================== */

    void Patrol()
    {
        agent.speed = walkSpeed;
        agent.stoppingDistance = 0f;
        agent.isStopped = false;

        ResetAnims();
        animator.SetBool(isWalk, true);

        agent.SetDestination(patrolTarget);

        if (!agent.pathPending && agent.remainingDistance <= 0.3f)
        {
            agent.isStopped = true;
            idleTimer = idleTime;
            state = State.Idle;
        }

        if (canSeePlayer)
            state = State.Chase;
    }

    void Idle()
    {
        ResetAnims();
        animator.SetBool(isLooking, true);

        idleTimer -= Time.deltaTime;
        transform.Rotate(Vector3.up * 60f * Time.deltaTime);

        if (idleTimer <= 0f)
        {
            SetNewPatrolPoint();
            state = State.Patrol;
        }

        if (canSeePlayer)
            state = State.Chase;
    }

    /* ===================== CHASE ===================== */

    void Chase()
    {
        if (!player)
        {
            InitializePatrol();
            return;
        }

        agent.speed = runSpeed;
        agent.stoppingDistance = attackDistance;
        agent.isStopped = false;

        ResetAnims();
        animator.SetBool(isRun, true);

        agent.SetDestination(player.position);
        FacePlayer(10f);

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackDistance)
        {
            agent.isStopped = true;
            state = State.Attack;
        }
    }

    /* ===================== ATTACK ===================== */

    void Attack()
    {
        if (!player)
        {
            InitializePatrol();
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        // Player moved away → chase again
        if (dist > attackDistance + 0.5f)
        {
            EndAttackImmediate();
            state = canSeePlayer ? State.Chase : State.Patrol;
            return;
        }

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        FacePlayer(15f);

        // FORCE movement animations OFF
        animator.SetBool(isRun, false);
        animator.SetBool(isWalk, false);
        animator.SetBool(isLooking, false);

        if (!attackInProgress && Time.time >= nextAttackTime)
        {
            currentAttack = Random.value < 0.5f ? isAtk1 : isAtk2;

            animator.SetBool(isAtk1, false);
            animator.SetBool(isAtk2, false);
            animator.SetBool(currentAttack, true);

            attackInProgress = true;
            nextAttackTime = Time.time + attackCooldown;
        }

        if (hitPlayer && playerHealth)
        {
            playerHealth.TakeDamage(damage);
            hitPlayer = false;
        }
    }

    /* ===================== HELPERS ===================== */

    void FacePlayer(float speed)
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * speed);
    }

    void SetNewPatrolPoint()
    {
        if (!searchArea)
        {
            patrolTarget = transform.position;
            return;
        }

        Bounds b = searchArea.bounds;
        patrolTarget = new Vector3(
            Random.Range(b.min.x, b.max.x),
            transform.position.y,
            Random.Range(b.min.z, b.max.z)
        );
    }

    void ResetAnims()
    {
        animator.SetBool(isWalk, false);
        animator.SetBool(isRun, false);
        animator.SetBool(isLooking, false);
        animator.SetBool(isAtk1, false);
        animator.SetBool(isAtk2, false);
    }

    void EndAttackImmediate()
    {
        animator.SetBool(isAtk1, false);
        animator.SetBool(isAtk2, false);
        currentAttack = null;
        attackInProgress = false;
        hitPlayer = false;
    }

    /* ===================== ANIMATION EVENTS ===================== */

    // Call from attack animation
    public void HitPlayer()
    {
        hitPlayer = true;
    }

    // Call at END of attack animation
    public void EndAttack()
    {
        EndAttackImmediate();
        state = canSeePlayer ? State.Chase : State.Patrol;
    }
}
