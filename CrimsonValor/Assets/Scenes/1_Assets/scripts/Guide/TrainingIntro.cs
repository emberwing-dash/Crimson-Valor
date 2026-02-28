using UnityEngine;
using UnityEngine.XR;
using System.Collections;

public class TrainingIntro : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueTyper dialogueTyper;

    [TextArea(2, 4)]
    [SerializeField] private string[] forcedDialogues;

    [Header("Enable After Dialogue")]
    [SerializeField] private GameObject audioInputObject;

    private bool playerInside;
    private bool hasTriggered;
    private bool lastBState;

    /* ---------------- TRIGGER ---------------- */

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    /* ---------------- UPDATE ---------------- */

    private void Update()
    {
        if (hasTriggered) return;
        if (!playerInside) return;

        if (IsBPressed())
        {
            StartIntro();
        }
    }

    /* ---------------- INTRO ---------------- */

    private void StartIntro()
    {
        hasTriggered = true;

        if (dialogueTyper != null && forcedDialogues.Length > 0)
        {
            dialogueTyper.ShowDialogueUI();
            dialogueTyper.StartDialogue(forcedDialogues);

            StartCoroutine(WaitForDialogueEnd());
        }
    }

    private IEnumerator WaitForDialogueEnd()
    {
        yield return new WaitUntil(() => dialogueTyper.IsDialogueFinished);

        if (audioInputObject != null)
            audioInputObject.SetActive(true);
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
