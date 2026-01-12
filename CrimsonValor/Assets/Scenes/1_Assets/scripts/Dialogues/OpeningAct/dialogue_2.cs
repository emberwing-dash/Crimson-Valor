using UnityEngine;

public class dialogue_2 : MonoBehaviour
{
    [SerializeField] private DialogueTyper dialogueTyper;

    [TextArea]
    [SerializeField] private string[] playerDialogueLines;
    [SerializeField] GameObject area;

    public void TriggerPlayerDialogue()
    {
        dialogueTyper.StartDialogue(playerDialogueLines);
        area.SetActive(true);
    }
}
