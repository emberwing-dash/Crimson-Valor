using UnityEngine;
using System.Collections;

public class GuideIntro : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueTyper dialogueTyper;

    [TextArea]
    [SerializeField] private string[] guideDialogueLines;

    [Header("Pointing")]
    [SerializeField] private Transform pointTarget;
    [SerializeField] private dialogue_2 playerDialogue;
    [SerializeField] private float pointDuration = 1.5f; // length of pointing anim

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        SetIdle();
    }

    public void TriggerDialogue()
    {
        StopAllCoroutines();

        animator.SetBool("isTalking", true);
        animator.SetBool("isPointing", false);

        dialogueTyper.StartDialogue(guideDialogueLines);
        StartCoroutine(GuideSequence());
    }

    IEnumerator GuideSequence()
    {
        // 1️⃣ wait for guide dialogue
        yield return new WaitUntil(() => dialogueTyper.IsDialogueFinished);

        // 2️⃣ talking → pointing
        animator.SetBool("isTalking", false);
        animator.SetBool("isPointing", true);

        FacePointTarget();

        // 3️⃣ wait for pointing animation duration
        yield return new WaitForSeconds(pointDuration);

        animator.SetBool("isPointing", false);

        // 4️⃣ START PLAYER DIALOGUE (CANNOT FAIL)
        if (playerDialogue != null)
            playerDialogue.TriggerPlayerDialogue();
        else
            Debug.LogError("PlayerDialogue reference missing");
    }

    void FacePointTarget()
    {
        if (pointTarget == null) return;

        Vector3 dir = pointTarget.position - transform.position;
        dir.y = 0f;

        if (dir != Vector3.zero)
            transform.forward = dir.normalized;
    }

    void SetIdle()
    {
        animator.SetBool("isTalking", false);
        animator.SetBool("isPointing", false);
    }
}
