using System.Collections;
using UnityEngine;

public class dialogue_1 : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueTyper dialogueTyper;
    [SerializeField] private VRVibrator haptics;
    [SerializeField] private GuideDialogueTrigger guideTrigger;

    void Start()
    {
        StartCoroutine(OpeningSequence());
    }

    IEnumerator OpeningSequence()
    {
        dialogueTyper.StartDialogue(new string[]
        {
            "Where am I?",
            "Why am I wounded?",
            "Is that a person over there?",
            "I should talk to her"
        });

        haptics.VibrateRight(40f, 2f);

        // Wait until opening dialogue fully finishes
        yield return new WaitUntil(() => dialogueTyper.IsDialogueFinished);

        // Enable guide interaction AFTER opening act
        guideTrigger.EnableInteraction();
    }
}
