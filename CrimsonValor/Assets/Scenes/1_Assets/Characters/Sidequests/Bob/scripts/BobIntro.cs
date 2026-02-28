using UnityEngine;
using UnityEngine.XR;

public class BobIntro : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueTyper dialogueTyper;
    [SerializeField] private string[] introDialogues;

    [Header("Next Stage")]
    [SerializeField] private BobQuery bobQuery;

    [Header("Animation")]
    [SerializeField] private BobAnim bobAnim;

    [Header("Input Gap")]
    [SerializeField] private float inputCooldown = 0.5f;

    private bool playerInRange;
    private bool introStarted;
    private bool introFinished;
    private bool lastBState;

    void Start()
    {
        bobAnim.SetIdle();
        dialogueTyper.ShowName(); // show name for intro
    }

    void Update()
    {
        if (!playerInRange) return;

        // ▶ START INTRO
        if (!introStarted && !introFinished && IsBPressed())
        {
            introStarted = true;
            dialogueTyper.ShowName();
            dialogueTyper.StartDialogue(introDialogues);
        }

        // ▶ INTRO FINISHED
        if (introStarted && dialogueTyper.IsDialogueFinished)
        {
            introStarted = false;
            introFinished = true;

            dialogueTyper.HideName();                // ✅ HIDE NAME
            bobQuery.EnableQueryAfterDelay(inputCooldown);
        }
    }

    /* ---------------- TRIGGER ---------------- */

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    /* ---------------- VR INPUT ---------------- */

    private bool IsBPressed()
    {
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (!rightHand.isValid) return false;

        bool pressed;
        rightHand.TryGetFeatureValue(CommonUsages.secondaryButton, out pressed);

        bool result = pressed && !lastBState;
        lastBState = pressed;

        return result;
    }
}
