using System.Collections;
using UnityEngine;

public class TutorialIntro : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueTyper dialogueTyper;

    [TextArea(2, 4)]
    [SerializeField] private string[] introDialogues;

    [Header("Disable At Start")]
    [SerializeField] private GameObject move;                 // player movement root
    [SerializeField] private GameObject[] voiceInputObjects;  // mic / voice input UI

    [Header("Enable After Dialogue")]
    [SerializeField] private GameObject[] enableAfterDialogue;

    private void Start()
    {
        // Disable movement
        if (move != null)
            move.SetActive(false);

        // Disable voice input
        foreach (var obj in voiceInputObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // Start intro dialogue
        if (dialogueTyper != null && introDialogues.Length > 0)
        {
            dialogueTyper.ShowDialogueUI();
            dialogueTyper.StartDialogue(introDialogues);

            StartCoroutine(WaitForDialogueToFinish());
        }
    }

    /* ---------------- WAIT ---------------- */

    private IEnumerator WaitForDialogueToFinish()
    {
        yield return new WaitUntil(() => dialogueTyper.IsDialogueFinished);

        // Enable movement
        if (move != null)
            move.SetActive(true);

        // Enable voice input
        foreach (var obj in voiceInputObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // Enable any extra objects
        foreach (var obj in enableAfterDialogue)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }
}
