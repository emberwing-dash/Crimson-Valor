using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[System.Serializable]
public class DialogueLine
{
    public string speaker; // "Neko" or "Player"

    [TextArea(2, 4)]
    public string dialogue;
}

public class NekoNavController : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Movement Root (DISABLED DURING CUTSCENE)")]
    [SerializeField] private GameObject moveRoot;

    [Header("Path Points")]
    [SerializeField] private Transform runTarget;
    [SerializeField] private Transform tiredTarget;

    [Header("Look / Point Target")]
    [SerializeField] private Transform pointLookTarget;
    [SerializeField] private float rotateSpeed = 6f;
    [SerializeField] private float pointAnimDuration = 2f;

    [Header("Distances")]
    [SerializeField] private float arriveDistance = 0.2f;

    [Header("Animator Parameters")]
    [SerializeField] private string runBool = "isRunning";
    [SerializeField] private string tiredBool = "isTired";
    [SerializeField] private string puffBool = "isPuff";
    [SerializeField] private string pointBool = "isPointing";

    [Header("Dialogue Typers")]
    [SerializeField] private DialogueTyper nekoDialogueTyper;
    [SerializeField] private DialogueTyper playerDialogueTyper;

    [Header("Dialogue Sequence")]
    [SerializeField] private DialogueLine[] dialogueSequence;

    [Header("Post-Cutscene Interaction")]
    [SerializeField] private NekoInteractionController nekoInteractionController;

    [Header("Loading Screen")]
    [SerializeField] private LoadScreen loadScreen;
    [SerializeField] private float loadDelay = 0.5f;

    void OnEnable()
    {
        if (moveRoot != null)
            moveRoot.SetActive(false);

        if (nekoInteractionController != null)
            nekoInteractionController.enabled = false;

        if (loadScreen != null)
            loadScreen.gameObject.SetActive(true);

        StartCoroutine(BeginFlow());
    }

    IEnumerator BeginFlow()
    {
        yield return new WaitForSeconds(loadDelay);

        if (loadScreen != null)
            loadScreen.gameObject.SetActive(false);

        yield return StartCoroutine(CutsceneRoutine());
    }

    /* -------------------- CUTSCENE -------------------- */

    IEnumerator CutsceneRoutine()
    {
        agent.isStopped = false;
        agent.stoppingDistance = arriveDistance;

        /* ---------- RUN ---------- */
        animator.SetBool(runBool, true);
        animator.SetBool(tiredBool, false);
        animator.SetBool(puffBool, false);
        animator.SetBool(pointBool, false);

        agent.SetDestination(runTarget.position);
        yield return WaitUntilArrived();

        /* ---------- TIRED ---------- */
        animator.SetBool(runBool, false);
        animator.SetBool(tiredBool, true);

        agent.SetDestination(tiredTarget.position);
        yield return WaitUntilArrived();

        /* ---------- PUFF ---------- */
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        animator.SetBool(tiredBool, false);
        animator.SetBool(puffBool, true);

        /* ---------- DIALOGUE ---------- */
        if (dialogueSequence != null && dialogueSequence.Length > 0)
        {
            foreach (var line in dialogueSequence)
            {
                DialogueTyper activeTyper = null;
                DialogueTyper inactiveTyper = null;

                if (line.speaker == "Neko")
                {
                    activeTyper = nekoDialogueTyper;
                    inactiveTyper = playerDialogueTyper;
                }
                else
                {
                    activeTyper = playerDialogueTyper;
                    inactiveTyper = nekoDialogueTyper;
                }

                if (inactiveTyper != null)
                    inactiveTyper.HideDialogueUI();

                activeTyper.ShowDialogueUI();
                activeTyper.StartDialogue(new string[] { line.dialogue });

                yield return new WaitUntil(() => activeTyper.IsDialogueFinished);
            }

            nekoDialogueTyper.HideDialogueUI();
            playerDialogueTyper.HideDialogueUI();
        }

        /* ---------- POINTING ---------- */
        yield return StartCoroutine(PointRoutine());

        /* ---------- IDLE ---------- */
        animator.SetBool(runBool, false);
        animator.SetBool(tiredBool, false);
        animator.SetBool(puffBool, false);
        animator.SetBool(pointBool, false);

        if (moveRoot != null)
            moveRoot.SetActive(true);

        if (nekoInteractionController != null)
            nekoInteractionController.enabled = true;
    }

    /* -------------------- POINTING -------------------- */

    IEnumerator PointRoutine()
    {
        if (pointLookTarget == null)
            yield break;

        animator.SetBool(puffBool, false);
        animator.SetBool(pointBool, true);

        float timer = 0f;

        while (timer < pointAnimDuration)
        {
            Vector3 dir = pointLookTarget.position - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    Time.deltaTime * rotateSpeed
                );
            }

            timer += Time.deltaTime;
            yield return null;
        }

        animator.SetBool(pointBool, false);
    }

    /* -------------------- HELPERS -------------------- */

    IEnumerator WaitUntilArrived()
    {
        while (agent.pathPending ||
               agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }
    }
}
