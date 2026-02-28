using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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

    IEnumerator CutsceneRoutine()
    {
        agent.isStopped = false;
        agent.stoppingDistance = arriveDistance;

        animator.SetBool(runBool, true);
        agent.SetDestination(runTarget.position);
        yield return WaitUntilArrived();

        animator.SetBool(runBool, false);
        animator.SetBool(tiredBool, true);
        agent.SetDestination(tiredTarget.position);
        yield return WaitUntilArrived();

        agent.isStopped = true;
        animator.SetBool(tiredBool, false);
        animator.SetBool(puffBool, true);

        foreach (var line in dialogueSequence)
        {
            DialogueTyper active = line.speaker == "Neko"
                ? nekoDialogueTyper
                : playerDialogueTyper;

            DialogueTyper inactive = active == nekoDialogueTyper
                ? playerDialogueTyper
                : nekoDialogueTyper;

            inactive.HideDialogueUI();
            active.ShowDialogueUI();
            active.StartDialogue(new[] { line.dialogue });

            yield return new WaitUntil(() => active.IsDialogueFinished);
        }

        nekoDialogueTyper.HideDialogueUI();
        playerDialogueTyper.HideDialogueUI();

        yield return StartCoroutine(PointRoutine());

        animator.SetBool(puffBool, false);
        animator.SetBool(pointBool, false);

        if (moveRoot != null)
            moveRoot.SetActive(true);

        if (nekoInteractionController != null)
            nekoInteractionController.enabled = true;
    }

    IEnumerator PointRoutine()
    {
        if (pointLookTarget == null) yield break;

        animator.SetBool(pointBool, true);

        float t = 0f;
        while (t < pointAnimDuration)
        {
            Vector3 dir = pointLookTarget.position - transform.position;
            dir.y = 0f;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * rotateSpeed
            );
            t += Time.deltaTime;
            yield return null;
        }

        animator.SetBool(pointBool, false);
    }

    IEnumerator WaitUntilArrived()
    {
        while (agent.pathPending ||
               agent.remainingDistance > agent.stoppingDistance)
            yield return null;
    }
}
