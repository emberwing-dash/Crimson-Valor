using System.Collections;
using TMPro;
using UnityEngine;

public class PaladinReward : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueTyper dialogueTyper;

    [TextArea(2, 4)]
    [SerializeField] private string[] rewardDialogues;

    [Header("Name UI")]

    [SerializeField] private TMP_Text nameObject;

    private void OnEnable()
    {
        
        if (dialogueTyper == null || rewardDialogues == null || rewardDialogues.Length == 0)
            return;

        if (nameObject != null)
        {
            nameObject.gameObject.SetActive(true);
            nameObject.text = "Player";
        }

            dialogueTyper.ShowDialogueUI();
        dialogueTyper.StartDialogue(rewardDialogues);

        StartCoroutine(WaitForDialogueEnd());
    }

    private IEnumerator WaitForDialogueEnd()
    {
        yield return new WaitUntil(() => dialogueTyper.IsDialogueFinished);

        dialogueTyper.HideDialogueUI();

        if (nameObject != null)
            nameObject.gameObject.SetActive(false);
    }
}
