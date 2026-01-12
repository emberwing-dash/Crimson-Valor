using UnityEngine;

public class ExampleDialogue : MonoBehaviour
{
    [SerializeField] DialogueTyper dialogueTyper;
    [SerializeField] AudioClip[] voiceClips;

    void Start()
    {
        dialogueTyper.StartDialogue(new string[]
    {
        "Test line one.",
        "If you see this, typing works."
    });
    }

}
