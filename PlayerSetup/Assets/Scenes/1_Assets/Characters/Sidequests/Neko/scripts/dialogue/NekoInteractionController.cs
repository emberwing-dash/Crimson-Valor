using UnityEngine;
using System.Collections;
using UnityEngine.XR;

[System.Serializable]
public class SpeakerDialogue
{
    public string speaker; // "Neko" or "Player"

    [TextArea(2, 4)]
    public string[] lines;
}

public class NekoInteractionController : MonoBehaviour
{
    [Header("Dialogue Typers")]
    [SerializeField] private DialogueTyper nekoDialogueTyper;
    [SerializeField] private DialogueTyper playerDialogueTyper;

    [Header("Dialogue Sets")]
    [SerializeField] private SpeakerDialogue[] dialogueSet;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string isTalk = "isTalk";
    [SerializeField] private string isListen = "isListen";
    [SerializeField] private string isPoint = "isPoint";

    [Header("Pointing & Rotation")]
    [SerializeField] private GameObject rotateRoot;          // 🔄 PARENT TO ROTATE
    [SerializeField] private GameObject pointTargetObject;   // 👉 OBJECT TO POINT AT
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float pointDuration = 1.2f;

    [Header("Post Conversation Object")]
    [SerializeField] private GameObject showAfterConversation;

    bool playerInside;
    bool interactionStarted;
    bool lastBState;

    void OnEnable()
    {
        GetComponent<BoxCollider>().isTrigger = true;

        playerInside = false;
        interactionStarted = false;
        lastBState = false;

        ResetAnimStates();

        if (showAfterConversation != null)
            showAfterConversation.SetActive(false);
    }

    /* ---------------- TRIGGER ---------------- */

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    /* ---------------- UPDATE ---------------- */

    void Update()
    {
        if (interactionStarted || !playerInside)
            return;

        if (IsBPressed())
        {
            interactionStarted = true;
            StartCoroutine(DialogueRoutine());
        }
    }

    /* ---------------- DIALOGUE ---------------- */

    IEnumerator DialogueRoutine()
    {
        foreach (var entry in dialogueSet)
        {
            DialogueTyper typer =
                entry.speaker == "Player"
                ? playerDialogueTyper
                : nekoDialogueTyper;

            if (typer == null || entry.lines == null || entry.lines.Length == 0)
                continue;

            SetTalkState(entry.speaker);

            typer.ShowDialogueUI();
            typer.StartDialogue(entry.lines);

            yield return new WaitUntil(() => typer.IsDialogueFinished);

            typer.HideDialogueUI();
            ResetAnimStates();
        }

        // 👉 After dialogue → point
        yield return StartCoroutine(PointRoutine());

        // 👉 Back to idle
        ResetAnimStates();

        // 👉 Show object
        if (showAfterConversation != null)
            showAfterConversation.SetActive(true);
    }

    /* ---------------- POINT & ROTATE ---------------- */

    IEnumerator PointRoutine()
    {
        if (rotateRoot == null || pointTargetObject == null)
            yield break;

        animator.SetBool(isPoint, true);

        Transform rotateTransform = rotateRoot.transform;
        Transform target = pointTargetObject.transform;

        float t = 0f;

        while (t < pointDuration)
        {
            t += Time.deltaTime;

            Vector3 dir = target.position - rotateTransform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                rotateTransform.rotation = Quaternion.Slerp(
                    rotateTransform.rotation,
                    targetRot,
                    Time.deltaTime * rotateSpeed
                );
            }

            yield return null;
        }

        animator.SetBool(isPoint, false);
    }

    /* ---------------- ANIMATION STATES ---------------- */

    void SetTalkState(string speaker)
    {
        if (speaker == "Player")
        {
            animator.SetBool(isListen, true);
            animator.SetBool(isTalk, false);
        }
        else
        {
            animator.SetBool(isTalk, true);
            animator.SetBool(isListen, false);
        }
    }

    void ResetAnimStates()
    {
        animator.SetBool(isTalk, false);
        animator.SetBool(isListen, false);
        animator.SetBool(isPoint, false);
    }

    /* ---------------- VR INPUT ---------------- */

    bool IsBPressed()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (!device.isValid) return false;

        bool pressed;
        device.TryGetFeatureValue(CommonUsages.secondaryButton, out pressed);

        bool result = pressed && !lastBState;
        lastBState = pressed;
        return result;
    }
}
