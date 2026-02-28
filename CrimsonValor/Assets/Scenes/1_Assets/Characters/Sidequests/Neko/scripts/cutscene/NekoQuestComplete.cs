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

public class NekoQuestComplete : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject playerMoveRoot; // DISABLE / ENABLE THIS

    [Header("Approach Settings")]
    [SerializeField] private float stopDistanceFromPlayer = 1.4f;

    [Header("Look / Point Target")]
    [SerializeField] private Transform pointLookTarget;
    [SerializeField] private float rotateSpeed = 6f;
    [SerializeField] private float pointAnimDuration = 2f;

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
    [SerializeField] private MonoBehaviour nekoInteractionController;

    void OnEnable()
    {
        StartCoroutine(CutsceneFlow());
    }

    IEnumerator CutsceneFlow()
    {
        /* ---------- SAFETY ---------- */
        if (!agent || !animator || !player)
            yield break;

        /* ---------- DISABLE PLAYER MOVE ---------- */
        if (playerMoveRoot != null)
            playerMoveRoot.SetActive(false);

        if (nekoInteractionController != null)
            nekoInteractionController.enabled = false;

        agent.isStopped = false;
        agent.stoppingDistance = stopDistanceFromPlayer;

        /* ---------- RUN TO PLAYER ---------- */
        animator.SetBool(runBool, true);
        animator.SetBool(tiredBool, false);
        animator.SetBool(puffBool, false);
        animator.SetBool(pointBool, false);

        Vector3 targetPos = player.position;
        agent.SetDestination(targetPos);

        yield return StartCoroutine(WaitUntilNearPlayer());

        /* ---------- PUFF / TIRED ---------- */
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        animator.SetBool(runBool, false);
        animator.SetBool(tiredBool, true);

        yield return new WaitForSeconds(1.2f);

        animator.SetBool(tiredBool, false);
        animator.SetBool(puffBool, true);

        yield return new WaitForSeconds(0.8f);

        /* ---------- DIALOGUE ---------- */
        if (dialogueSequence != null && dialogueSequence.Length > 0)
        {
            foreach (DialogueLine line in dialogueSequence)
            {
                DialogueTyper activeTyper;
                DialogueTyper inactiveTyper;

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

        /* ---------- END CUTSCENE ---------- */
        animator.SetBool(runBool, false);
        animator.SetBool(tiredBool, false);
        animator.SetBool(puffBool, false);
        animator.SetBool(pointBool, false);

        if (playerMoveRoot != null)
            playerMoveRoot.SetActive(true);

        if (nekoInteractionController != null)
            nekoInteractionController.enabled = true;
    }

    /* ================= HELPERS ================= */

    IEnumerator WaitUntilNearPlayer()
    {
        while (agent.pathPending ||
               agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }
    }

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
}
